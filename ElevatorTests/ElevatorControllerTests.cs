using ElevatorAction;
using ElevatorAction.UserInterface;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using ElevatorTests.MockObjects;

namespace ElevatorTests
{
    [TestClass]
    public class ElevatorControllerTests
    {
        private TestInterface _ui = new TestInterface();
        private int _retryCount = 0;
        private ElevatorController elevatorController;

        private void init()
        {
            _ui.Reset();

            if (elevatorController is null)
                elevatorController = new ElevatorController(_ui, _retryCount);

            if (_retryCount == 0)
                _retryCount = TestUtils.nrRetries;
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

        public void ElevatorControllerRetriesThenExits()
        {
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

        //Template
        #region  TESTS

        [TestMethod]
        public void myTest()
        {
            init();


        }

        #endregion TESTS

    }
}