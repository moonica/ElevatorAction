using ElevatorAction.UserInterface;
using ElevatorAction.Models;
using Microsoft.Extensions.Configuration;

namespace ElevatorAction
{
    public class ElevatorMaster
    {
        private IUserInterface _ui;
        private readonly IConfiguration _config;
        private readonly int _retryCount = 5; //default
        private ElevatorController controller;
        private Dictionary<string, string> _phrases = new Dictionary<string, string>();

        private readonly List<string> affirmativeInputs = new List<string> { "YES", "Y" };

        public ElevatorMaster(IUserInterface ui, IConfiguration config, Dictionary<string, string> phrases)
        {
            _ui = ui;
            _config = config;

            //Get all phrases from the utils for now, this could come from translation files later
            _phrases = phrases;

            //override the retry count with the config value, if present, else leave at the default
            int.TryParse(Utils.GetConfigSetting(_config, "retryCount"), out _retryCount);

            controller = new ElevatorController(_ui, _phrases, _retryCount);
        }

        internal async Task Execute()
        {
            _ui.Display(_phrases["Welcome"]);

            //get first command
            var input = await _ui.GetCommandAsync();

            //for as long as the user is entering commands (that aren't to exit), process them
            while (input != CommandType.Exit)
            {
                await PerformCommand(input);

                //once the command has been performed, wait for the next command
                input = await _ui.GetCommandAsync();
            }

            //once no more commands are received, exit (no need to await this operation)
            controller.PerformShutdownWithFinalMessage(_phrases["GOODBYE"]);
        }

        public async Task PerformCommand(CommandType command)
        {
            switch (command)
            {
                case CommandType.TryAgain:
                    await PerformCommand(
                        await controller.GetCommandAfterRetry(_phrases["Retry"], _phrases["RetryCountdown"])
                        );
                    break;
                case CommandType.Exit:
                    await controller.PerformShutdownWithConfirmation(_phrases["AreYouSure"]);
                    break;
                case CommandType.Abort:
                default:
                    _ui.ShutDown();
                    break;
            }
        }
    }
}