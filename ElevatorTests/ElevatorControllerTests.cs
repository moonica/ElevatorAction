using ElevatorAction;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using ElevatorTests.MockObjects;
using ElevatorAction.Models;

namespace ElevatorTests
{
    [TestClass]
    public class ElevatorControllerTests
    {
        private TestInterface _ui = new TestInterface();
        private int _retryCount = 0;
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
                if (_retryCount == 0)
                    _retryCount = TestUtils.nrRetries;
            }

            //if elevator controller is not yet initialised, or a new retry count has been specified, initialise the controller 
            if ((elevatorController is null) || retryCount.HasValue)
                elevatorController = new ElevatorController(_ui, _retryCount);
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
        public async void GetCommandAfterRetry_NoRetryMax_ReturnsValidCommandAfterManyTries()
        {
            init(-1); //no max retries

            _ui.TestInput = "lorem ipsum"; //invalid input
            CommandType commandReturned;
            int commandCounter = 0;

            //retry many times without being kicked out, since retries should be unlimited
            for (commandCounter = 0; commandCounter < 20; commandCounter++)
            {
                commandReturned = await elevatorController.GetCommandAfterRetry(Utils.Phrases_en["Retry"], Utils.Phrases_en["RetryCountdown"]);

                TestUtils.assertCommandsMatchHelper(CommandType.TryAgain, commandReturned);
            }

            Assert.IsTrue(commandCounter == 20, $"With no retry max set, we should have seen 20 retry attempts. Actual value: {commandCounter}");

            //finish on a valid input to exit the retry cycle
            _ui.TestInput = "TEST"; 

            commandReturned = await elevatorController.GetCommandAfterRetry(Utils.Phrases_en["Retry"], Utils.Phrases_en["RetryCountdown"]);

            TestUtils.assertCommandsMatchHelper(CommandType.Test, commandReturned);

            _ui.Reset();
        }

        public async void GetCommandAfterRetry_RetryMaxZero_Aborts()
        {
            init(0); //zero max retries (not valid value)

            _ui.TestInput = "lorem ipsum"; //doesn't matter
                                           
            var commandReturned = await elevatorController.GetCommandAfterRetry(Utils.Phrases_en["Retry"], Utils.Phrases_en["RetryCountdown"]);

            TestUtils.assertCommandsMatchHelper(CommandType.Abort, commandReturned);

            _ui.Reset();
        }
        //init();
        //_ui.SetMultipleCommands
        //    (
        //        new List<CommandType>
        //        {
        //            CommandType.TryAgain,
        //            CommandType.Exit
        //        }
        //    );

        //elevatorMaster.PerformCommand(CommandType.TryAgain);

        //if ((_ui.outputs?.Count ?? 0) < 3)
        //{
        //    Assert.Fail(string.Format(errMsgExpectedOutputs, 3, _ui.outputs?.Count));
        //}
        //else
        //{
        //    TestUtils.assertOutputsPartialMatchHelper(0, _ui.outputs[0], Utils.Phrases_en["Retry"], CommandType.TryAgain, 80);

        //    TestUtils.assertOutputsPartialMatchHelper(1, _ui.outputs[1], Utils.Phrases_en["Retry"], CommandType.TryAgain, 80);

        //    TestUtils.assertOutputsPartialMatchHelper(2, _ui.outputs[2], Utils.Phrases_en["AreYouSure"], CommandType.Exit, 80);
        //}

        //_ui.Reset();

        //}

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

        //Template
        #region  TESTS

        //[TestMethod]
        //public void myTest()
        //{
        //    init();


        //}

        #endregion TESTS

    }
}