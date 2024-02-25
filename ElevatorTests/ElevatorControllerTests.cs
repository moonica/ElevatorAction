using ElevatorAction;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using ElevatorTests.MockObjects;
using ElevatorAction.Models;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

namespace ElevatorTests
{
    [TestClass]
    public class ElevatorControllerTests
    {
        private TestInterface _ui = new TestInterface();
        private TestLogger _log = new TestLogger();
        private int _retryCount = -2;
        private ElevatorController elevatorController;

        private void init(int? retryCount = null)
        {
            _ui.Reset();

            //if the retrycount is being set from the caller, use that value
            if (retryCount.HasValue)
                _retryCount = retryCount.Value;
            else
            {
                //if retrycount was not already initialised (is still default value), use the "config" value
                if (_retryCount == -2)
                    _retryCount = TestUtils.nrRetries;
            }

            //if elevator controller is not yet initialised, or a new retry count has been specified, initialise the controller 
            if ((elevatorController is null) || retryCount.HasValue)
                elevatorController = new ElevatorController(_ui, _log, _retryCount);
        }

        #region VALIDATION TESTS

        [TestMethod]
        public void ValidateRetryCountValidatesValidValues()
        {
            init();

            List<int?> valuesToTest = new List<int?>() { -1, int.MaxValue - 1, 1, 2, 10, 100 };

            foreach (var val in valuesToTest)
            {
                Assert.IsTrue(elevatorController.validateRetryCount(val), $"Valid value {val} rejected by ElevatorController.validateRetryCount");
            }
        }

        [TestMethod]
        public void ValidateRetryCountFailsOnInvalidValues()
        {
            init();

            List<int?> valuesToTest = new List<int?>() { null, -2, 0, int.MaxValue };

            foreach (var val in valuesToTest)
            {
                Assert.IsFalse(elevatorController.validateRetryCount(val), $"Invalid value {val} managed to get past ElevatorController.validateRetryCount");
            }
        }

        #endregion VALIDATION TESTS


        #region RETRY TESTS

        [TestMethod]
        public void GetCommandAfterRetry_NoRetryMax_ReturnsValidCommandAfterManyTries()
        {
            init(-1); //no max retries

            CommandType commandReturned;

            _ui.multipleCommands = true;
            var commands = new CommandType[21];
            Array.Fill(commands, CommandType.TryAgain, 0, 20);
            commands[20] = CommandType.Test;

            _ui.SetMultipleCommands(commands.ToList());

            //retry many times without being kicked out, since retries should be unlimited. The internal while loop will only stop because the final input is a valid command ("Test")
            commandReturned = elevatorController.GetCommandAfterRetry(Utils.Phrases_en["Retry"], Utils.Phrases_en["RetryCountdown"]).Result;

            TestUtils.assertCommandsMatchHelper(CommandType.Test, commandReturned);

            var outputCount = _ui.outputs?.Count;
            Assert.AreEqual(21, outputCount, $"Expected 21 outputs to the ui; found {outputCount}");

            Assert.IsTrue(_ui.outputs?.All(output => output.Equals(Utils.Phrases_en["Retry"])), $"Expected all ui outputs to contain retry message");

            _ui.Reset();
        }

        [TestMethod]
        public void GetCommandAfterRetry_RetryMaxZero_ThrowsException()
        {
            init(0); //zero max retries (not valid value)

            _ui.TestInput = "lorem ipsum"; //doesn't matter

            try
            {
                var cmd = elevatorController.GetCommandAfterRetry(string.Empty, string.Empty).Result;
            }
            catch (Exception ex)
            {
                var isArgumentEx = ex.GetType() == typeof(ArgumentException);
                isArgumentEx |= ex.InnerException.GetType() == typeof(ArgumentException);

                Assert.IsTrue(isArgumentEx);
                return;
            }

            _ui.Reset();

            //if we got this far, the test has failed
            Assert.Fail("End of test reached; expected an ArgumentException exception");
        }

        [TestMethod]
        public void GetCommandAfterRetry_WithinRetryLimit_ReturnsValidCommand()
        {
            init(3);

            _ui.TestCommand = CommandType.Test; //valid command

            var commandReturned = elevatorController.GetCommandAfterRetry(Utils.Phrases_en["Retry"], Utils.Phrases_en["RetryCountdown"]).Result;

            TestUtils.assertCommandsMatchHelper(CommandType.Test, commandReturned);

            _ui.Reset();
        }

        [TestMethod]
        public void GetCommandAfterRetry_WithinRetryLimitWithInvalidCommand_ReturnsAbort()
        {
            init(3);

            _ui.TestCommand = CommandType.TryAgain; //invalid command for exiting the while loop

            var commandReturned = elevatorController.GetCommandAfterRetry(Utils.Phrases_en["Retry"], Utils.Phrases_en["RetryCountdown"]).Result;

            TestUtils.assertCommandsMatchHelper(CommandType.Abort, commandReturned);

            _ui.Reset();
        }

        [TestMethod]
        public void GetCommandAfterRetry_ReachedRetryLimit_ReturnsValidCommand()
        {
            init(3);
            CommandType returnCommand;

            //set 2 retries to match the max retry count of 3, and then a valid command (since the first time we call the function counts as a "retry")
            var inputCommands = new List<CommandType>
                    {
                        CommandType.TryAgain,
                        CommandType.TryAgain,
                        CommandType.Test
                    };
            _ui.SetMultipleCommands(inputCommands);

            returnCommand = elevatorController.GetCommandAfterRetry(Utils.Phrases_en["Retry"], Utils.Phrases_en["RetryCountdown"]).Result;

            //check we retried 3 times
            var outputCount = _ui.outputs?.Count ?? 0;
            if (outputCount != 3)
                Assert.Fail($"Expected 4 outputs, received {outputCount}");

            //first three outputs should be retry prompts
            var nrRetryOutputs = _ui.outputs?.Where(s => s.Contains(Utils.Phrases_en["Retry"]))?.Count();
            Assert.IsTrue(nrRetryOutputs == 3, $"Expected 3 retry prompt outputs; received {nrRetryOutputs}");

            //check the final result was a "TEST"
            Assert.IsTrue(returnCommand == CommandType.Test, $"Final command expected to be TEST; received '{returnCommand}' instead");
        }


        [TestMethod]
        public void GetCommandAfterRetry_ReachedRetryLimitWithInvalidCommand_ReturnsAbort()
        {
            init(3);
            CommandType returnCommand;

            //set 2 retries to match the max retry count of 3, and then an valid command that just so happens to also be "retry" (since the first time we call the function counts as a "retry"). The function doesn't take text input, and there is no automatically failing command. Retries trigger the same logic though.
            var inputCommands = new List<CommandType>
                    {
                        CommandType.TryAgain,
                        CommandType.TryAgain,
                        CommandType.TryAgain
                    };
            _ui.SetMultipleCommands(inputCommands);

            returnCommand = elevatorController.GetCommandAfterRetry(Utils.Phrases_en["Retry"], Utils.Phrases_en["RetryCountdown"]).Result;

            //check we retried 3 times
            var outputCount = _ui.outputs?.Count ?? 0;
            if (outputCount != 3)
                Assert.Fail($"Expected 4 outputs, received {outputCount}");

            //first three outputs should be retry prompts
            var nrRetryOutputs = _ui.outputs?.Where(s => s.Contains(Utils.Phrases_en["Retry"]))?.Count();
            Assert.IsTrue(nrRetryOutputs == 3, $"Expected 3 retry prompt outputs; received {nrRetryOutputs}");

            //check the final result was ABORT
            Assert.IsTrue(returnCommand == CommandType.Abort, $"Final command expected to be ABORT; received '{returnCommand}' instead");
        }


        [TestMethod]
        public void GetCommandAfterRetry_ExceedRetryLimit_ReturnsAbort()
        {
            init(3);
            CommandType returnCommand;

            //set 3 retries to exceed the max retry count of 3 (since the first time we call the function counts as a "retry"), then a valid command we should never hit
            var inputCommands = new List<CommandType>
                    {
                        CommandType.TryAgain,
                        CommandType.TryAgain,
                        CommandType.TryAgain,
                        CommandType.Test
                    };
            _ui.SetMultipleCommands(inputCommands);

            returnCommand = elevatorController.GetCommandAfterRetry(Utils.Phrases_en["Retry"], Utils.Phrases_en["RetryCountdown"]).Result;

            //check we retried 3 times
            var outputCount = _ui.outputs?.Count ?? 0;
            if (outputCount != 3)
                Assert.Fail($"Expected 4 outputs, received {outputCount}");

            //all outputs should be retry prompts
            Assert.IsTrue(_ui.outputs?.All(s => s.Contains(Utils.Phrases_en["Retry"])), $"Expected all outputs to be retry prompts");

            //check the final result was an abort, not a test
            Assert.IsTrue(returnCommand == CommandType.Abort, $"Final command expected to be ABORT; received '{returnCommand}' instead");
        }

        #endregion RETRY TESTS


        #region SHUTDOWN TESTS

        [TestMethod]
        public void ElevatorControllerExitsOnValidConfirmationInput()
        {
            init();
            List<string> inputs = new List<string> { "Y", "YES", "Yes", "yes", "y" };

            foreach (var input in inputs)
            {
                TestUtils.assertExitOutputsHelper<string>(_ui, (string s) => elevatorController.PerformShutdownWithConfirmation(Utils.Phrases_en["AreYouSure"]), Utils.Phrases_en["AreYouSure"], input, Utils.Phrases_en["AreYouSure"]);
            }
        }

        [TestMethod]
        public void ElevatorControllerDoesntExitOnInvalidConfirmationInput()
        {
            init();
            List<string> inputs = new List<string> { "n", "no", "N", "NO", "No", null, "loremipsum", "yess" };

            foreach (var input in inputs)
            {
                TestUtils.assertExitOutputsHelper<string>(_ui, (string s) => elevatorController.PerformShutdownWithConfirmation(Utils.Phrases_en["AreYouSure"]), Utils.Phrases_en["AreYouSure"], input, Utils.Phrases_en["AreYouSure"], false);
            }

            //no extra messages were written to the output list, and no outputs contain the shutdown message (ie, we never exited)
            Assert.AreEqual(_ui.outputs?.Count ?? 0, inputs.Count);
            Assert.IsFalse(_ui.outputs?.Contains(TestInterface.ExitString) ?? true);
        }

        #endregion SHUTDOWN TESTS

        /*
        Template
        #region  TESTS

        [TestMethod]
        public void myTest()
        {
            init();


        }
        
        #endregion TESTS
        */
    }
}