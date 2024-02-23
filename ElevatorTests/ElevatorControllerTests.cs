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


        #region  TESTS

        [TestMethod]
        public void myTest()
        {
            init();

            
        }

        #endregion TESTS
    }
}