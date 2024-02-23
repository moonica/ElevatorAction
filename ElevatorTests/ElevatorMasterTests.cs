using ElevatorAction;
using ElevatorAction.UserInterface;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using ElevatorTests.MockObjects;

namespace ElevatorTests
{
    [TestClass]
    public class ElevatorMasterTests
    {
        private IConfiguration _config = new TestConfig();
        private TestInterface _ui = new TestInterface();
        private int _retryCount = 0;
        private ElevatorMaster elevatorMaster;

        private string errMsgExpectedOutputs = "Expected {0} ui outputs; received {1}";
        

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
        public void ElevatorMasterSwitchKeepsRetryingUntilExit()
        {
            init();

            //set up test interface
            _ui.TestInput = "Y";
            _ui.SetMultipleCommands
                (
                    new List<CommandType>
                    {
                        CommandType.TryAgain, //second retry
                        CommandType.Exit
                    }
                );

            elevatorMaster.PerformCommand(CommandType.TryAgain); //first retry

            if ((_ui.outputs?.Count ?? 0) < 3)
            {
                Assert.Fail(string.Format(errMsgExpectedOutputs, 3, _ui.outputs?.Count));
            }
            else
            {
                List<Tuple<string, CommandType>> outputsAndCommandsToTest = new List<Tuple<string, CommandType>>()
                {
                    new Tuple<string, CommandType>(Utils.Phrases_en["Retry"], CommandType.TryAgain),
                    new Tuple<string, CommandType>(Utils.Phrases_en["Retry"], CommandType.TryAgain),
                    new Tuple<string, CommandType>(Utils.Phrases_en["AreYouSure"], CommandType.Exit),
                    new Tuple<string, CommandType>(TestInterface.ExitString, CommandType.Exit),
                };

                for (int i = 0; i < 4; i++)
                {
                    TestUtils.assertOutputsPartialMatchHelper(i, _ui.outputs[i], outputsAndCommandsToTest[i].Item1, outputsAndCommandsToTest[i].Item2, 50);
                }
            }

            _ui.Reset();
        }

        [TestMethod]
        public void ElevatorMasterRetriesMaxRetryTimes()
        {
            init();

            //set up test interface
            _ui.TestCommand = CommandType.TryAgain;

            elevatorMaster.PerformCommand(CommandType.TryAgain);

            if ((_ui.outputs?.Count ?? 0) < _retryCount)
            {
                Assert.Fail(string.Format(errMsgExpectedOutputs, _retryCount, _ui.outputs?.Count));
            }
            else
            {
                var outputCount = _ui.outputs.Count;
                for (int i = 0; i < outputCount - 1; i++)
                {
                    //the first n-1 outputs should be retry messages
                    TestUtils.assertOutputsPartialMatchHelper(i, _ui.outputs[i], Utils.Phrases_en["Retry"], CommandType.TryAgain, 30);

                    //the retries countdown timer should be correct
                    Assert.IsTrue(_ui.outputs[i].Contains(string.Format(Utils.Phrases_en["RetryCountdown"], _retryCount - i - 1)));

                }

                //the last output should be abort message
                TestUtils.assertOutputsPartialMatchHelper(outputCount, _ui.outputs
                    .Last(), TestInterface.ExitString, CommandType.Abort, 80);
            }

            _ui.Reset();
        }
        #endregion TRY AGAIN TESTS

        #region EXIT TESTS

        [TestMethod]
        public void ElevatorMasterSwitchSelectsCorrectCommand_exit()
        {
            init();
            _ui.TestInput = "YES";

            elevatorMaster.PerformCommand(CommandType.Exit);

            //Opting for "IsTrue" over "AreEqual", as the latter doesn't allow for custom messages

            int? outputCount = _ui.outputs?.Count;
            Assert.IsTrue(int.Equals(2, (outputCount ?? 0)), string.Format(errMsgExpectedOutputs, 2, outputCount));

            var firstOutput = _ui.outputs?.FirstOrDefault();
            Assert.IsTrue(string.Equals(Utils.Phrases_en["AreYouSure"], firstOutput), $"Last statement in output list expected was '{Utils.Phrases_en["AreYouSure"]}'; received instead: '{firstOutput}'");

            var lastOutput = _ui.outputs?.Last();
            Assert.IsTrue(string.Equals(TestInterface.ExitString, lastOutput), $"Last statement in output list expected was '{TestInterface.ExitString}'; received instead: '{lastOutput}'");
        }

        [TestMethod]
        public void ElevatorMasterSwitchSelectsCorrectCommand_abort()
        {
            init();

            elevatorMaster.PerformCommand(CommandType.Abort);

            //there shouldn't be a confirmation message for Abort like there is for Exit
            int? outputCount = _ui.outputs?.Count;
            Assert.IsTrue(int.Equals(1, (outputCount ?? 0)), string.Format(errMsgExpectedOutputs, 1, outputCount));

            var firstOutput = _ui.outputs?.FirstOrDefault();
            Assert.IsTrue(TestInterface.ExitString.Equals(firstOutput), $"Expected shutdown indicator; received '{firstOutput}'");
        }

        #endregion EXIT TESTS
    }
}