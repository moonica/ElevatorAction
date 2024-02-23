using ElevatorAction;
using ElevatorAction.UserInterface;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace ElevatorTests
{
    [TestClass]
    public class ElevatorMasterTests
    {
        private IConfiguration _config = new TestConfig();
        private TestInterface _ui = new TestInterface();
        private int _retryCount = 0;
        private ElevatorMaster elevatorMaster;

        private void init()
        {
            _ui.Reset();

            if (elevatorMaster is null)
                elevatorMaster = new ElevatorMaster(_ui, _config, Utils.Phrases_en);
            
            if (_retryCount == 0)
                _retryCount = TestUtils.nrRetries;
        }

        #region TRY AGAIN TESTS

        [TestMethod]
        public void ElevatorMasterRetriesThenExitsOn_retry_exit()
        {
            init();

            //set up test interface
            _ui.SetMultipleCommands
                (
                    new List<CommandType>
                    {
                        CommandType.TryAgain,
                        CommandType.Exit
                    }
                );

            elevatorMaster.PerformCommand(CommandType.TryAgain);

            if ((_ui.outputs?.Count ?? 0) < 3)
            {
                Assert.Fail($"Too few outputs detected from ElevatorMaster; it should have run 3 times; {_ui.outputs?.Count} outputs were received");
            }
            else
            {
                TestUtils.assertOutputsPartialMatchHelper(0, _ui.outputs[0], Utils.Phrases_en["Retry"], CommandType.TryAgain, 80);

                TestUtils.assertOutputsPartialMatchHelper(1, _ui.outputs[1], Utils.Phrases_en["Retry"], CommandType.TryAgain, 80);

                TestUtils.assertOutputsPartialMatchHelper(2, _ui.outputs[2], Utils.Phrases_en["AreYouSure"], CommandType.Exit, 80);
            }

            _ui.Reset();

        }

        [TestMethod]
        public void ElevatorMasterRetriesMaxTimes_retries()
        {
            init();

            //set up test interface
            _ui.TestCommand = CommandType.TryAgain;

            elevatorMaster.PerformCommand(CommandType.TryAgain);

            if ((_ui.outputs?.Count ?? 0) < _retryCount)
            {
                Assert.Fail($"Too few outputs detected from ElevatorMaster; it should have run the max number of times ({_retryCount}); {_ui.outputs?.Count} outputs were received");
            }
            else
            {
                var outputCount = _ui.outputs.Count;
                for (int i = 0; i < outputCount - 1; i++)
                {
                    //the first n-1 outputs should be retry messages
                    TestUtils.assertOutputsPartialMatchHelper(i, _ui.outputs[i], Utils.Phrases_en["Retry"], CommandType.TryAgain, 30);

                    //the retries remaining should be correct
                    Assert.IsTrue(_ui.outputs[i].Contains(string.Format(Utils.Phrases_en["RetryCountdown"], _retryCount - i - 1)));

                }

                //the last output should be exit/abort message
                TestUtils.assertOutputsPartialMatchHelper(outputCount, _ui.outputs
                    .Last(), TestInterface.ExitString, CommandType.Abort, 80);
            }

            _ui.Reset();
        }
        #endregion TRY AGAIN TESTS

        #region EXIT TESTS

        [TestMethod]
        public void ElevatorMasterExitsOnExitWithConfirmation_exit_yes()
        {
            init();
            List<string> inputs = new List<string> { "Y", "YES", "Yes", "yes", "y"};

            foreach (var input in inputs)
            {
                TestUtils.assertExitOutputsHelper(_ui, (CommandType) =>  elevatorMaster.PerformCommand(CommandType.Exit), input, Utils.Phrases_en["AreYouSure"]);
            }
        }

        [TestMethod]
        public void ElevatorMasterDoesntExitWithConfirmationOn_exit_no()
        {
            init();
            List<string> inputs = new List<string> { "n", "no", "N", "NO", "No", null, "loremipsum", "yess"};

            foreach (var input in inputs)
            {
                TestUtils.assertExitOutputsHelper(_ui, (CommandType) => elevatorMaster.PerformCommand(CommandType.Exit), input, Utils.Phrases_en["AreYouSure"], false);
            }

            //no extra messages were written to the output list, and no outputs contain the shutdown message (ie, we never exited)
            Assert.AreEqual(_ui.outputs?.Count ?? 0, inputs.Count);
            Assert.IsFalse(_ui.outputs?.Contains(TestInterface.ExitString) ?? true);
        }

        [TestMethod]
        public void ElevatorMasterExitsWithoutConfirmationOn_abort()
        {
            init();

            elevatorMaster.PerformCommand(CommandType.Abort);

            int? outputCount = _ui.outputs?.Count;
            Assert.IsTrue(int.Equals( 1, (outputCount ?? 0)), $"Expected one ui output; received {outputCount}");

            var firstOutput = _ui.outputs?.FirstOrDefault();
            Assert.IsTrue(TestInterface.ExitString.Equals(firstOutput), $"Expected shutdown indicator; received '{firstOutput}'");
        }

        #endregion EXIT TESTS
    }
}