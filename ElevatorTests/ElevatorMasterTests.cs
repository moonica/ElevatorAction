using ElevatorAction;
using ElevatorAction.UserInterface;
using Microsoft.Extensions.Configuration;

namespace ElevatorTests
{
    [TestClass]
    public class ElevatorMasterTests
    {
        private IConfiguration _config = new TestConfig();
        private TestInterface _ui = new TestInterface();
        private int _retryCount;

        private ElevatorMaster elevatorMaster;

        private void init()
        {
            elevatorMaster = new ElevatorMaster(_ui, _config);
            _retryCount = TestUtils.nrRetries;
        }

        #region COMMAND SWITCH TESTS

        [TestMethod]
        public void ElevatorMasterPerformsTryAgainThenExits()
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
                Assert.AreEqual(3, _ui.outputs.Count, $"PerformCommand should have run 3 times; it ran {_ui.outputs.Count} times instead");

                TestUtils.assertOutputsHelper(0, _ui.outputs[0], Utils.Phrases_en["Retry"], CommandType.TryAgain, 80);

                TestUtils.assertOutputsHelper(1, _ui.outputs[1], Utils.Phrases_en["Retry"], CommandType.TryAgain, 80);

                TestUtils.assertOutputsHelper(2, _ui.outputs[2], Utils.Phrases_en["AreYouSure"], CommandType.Exit, 80);
            }

            _ui.Reset();
            
        }

        #endregion COMMAND SWITCH TESTS
    }
}