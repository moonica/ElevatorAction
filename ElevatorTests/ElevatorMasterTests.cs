using ElevatorAction;
using ElevatorAction.UserInterface;
using Microsoft.Extensions.Configuration;

namespace ElevatorTests
{
    [TestClass]
    public class ElevatorMasterTests
    {
        //private IUserInterface ui = new TestInterface();
        private TestInterface ui = new TestInterface();
        private readonly IConfiguration _config;
        private int _retryCount = 2;

        private ElevatorMaster elevatorMaster;

        public ElevatorMasterTests(IConfiguration config)
        {
            _config = config;
            elevatorMaster = new ElevatorMaster(ui, _config);

            int.TryParse(Utils.GetConfigSetting(_config, "retryCount"), out _retryCount);
        }

        #region COMMAND SWITCH TESTS

        [TestMethod]
        public async void elevatorMasterPerformsTryAgainThenExits()
        {
            //set up test interface
            ui.SetMultipleCommands
                (
                    new List<CommandType>
                    {
                        CommandType.TryAgain,
                        CommandType.Exit
                    }
                );

            await elevatorMaster.PerformCommand(CommandType.TryAgain);

            if ((ui.outputs?.Count ?? 0) < 3)
            {
                Assert.Fail($"Too few outputs detected from ElevatorMaster; it should have run 3 times; {ui.outputs?.Count} outputs were received");
            }
            else
            {
                var err = "Command number {0} should have been {1}. Instead the output was '{2}'";
                Assert.AreEqual(3, ui.outputs.Count, $"PerformCommand should have run 3 times; it ran {ui.outputs.Count} times instead");

                Assert.AreEqual(ui.outputs[0], Utils.Phrases_en["Retry"], string.Format(err, 0, "Retry", ui.outputs[0]));
                Assert.AreEqual(ui.outputs[1], Utils.Phrases_en["Retry"], string.Format(err, 1, "Retry", ui.outputs[1]));
                Assert.AreEqual(ui.outputs[2], Utils.Phrases_en["Retry"], string.Format(err, 2, "Retry", ui.outputs[2]));
            }

            ui.Reset();
        }



        #endregion COMMAND SWITCH TESTS

    }
}