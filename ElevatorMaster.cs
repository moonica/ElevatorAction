using ElevatorAction.UserInterface;
using ElevatorAction.Models;
using Microsoft.Extensions.Configuration;

namespace ElevatorAction
{
    internal class ElevatorMaster
    {
        private IUserInterface _ui;
        private readonly IConfiguration _config;
        private readonly int _retryCount = 5; //default
        private ElevatorController controller;

        private readonly List<string> affirmativeInputs = new List<string> { "YES", "Y" };

        //All natural language phrases are enumerated here for ease of change; they could also be put in a localization dictionary per language
        private Dictionary<string, string> PHRASES = new Dictionary<string, string>
        {
            { "AreYouSure", "Are you sure you want to exit?" },
            { "Goodbye", "Goodbye" },
            { "RetriesExceeded", "You have entered too many invalid commands, the program will be exited." },
            { "Retry", "That command was not recognized. Please try again, or enter 'Help' to see a list of valid commands." },
            { "RetryCountdown", "{0} retries remaining..." },
            { "Welcome", "Welcome to Elevator Action. What would you like to do today?" }
        };

        internal ElevatorMaster(IUserInterface ui, IConfiguration config)
        {
            _ui = ui;
            _config = config;

            //override the retry count with the config value, if present, else leave at the default
            int.TryParse(Utils.GetConfigSetting(_config, "retryCount"), out _retryCount);

            controller = new ElevatorController(_ui, PHRASES, _retryCount);
        }

        internal async Task Execute()
        {
            _ui.Display(PHRASES["Welcome"]);

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
            controller.PerformShutdownWithFinalMessage(PHRASES["GOODBYE"]);
        }

        private async Task PerformCommand(CommandType command)
        {
            switch (command)
            {
                case CommandType.TryAgain:
                    await PerformCommand(
                        await controller.GetCommandAfterRetry(PHRASES["Retry"], PHRASES["RetryCountdown"])
                        );
                    break;
                case CommandType.Exit:
                    await controller.PerformShutdownWithConfirmation(PHRASES["AreYouSure"]);
                    break;
                case CommandType.Abort:
                default:
                    _ui.ShutDown();
                    break;
            }
        }


    }
}