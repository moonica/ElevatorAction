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

        private readonly List<string> affirmativeInputs = new List<string> { "YES", "Y" };

        //All natural language phrases are enumerated here for ease of change; they could also be put in a localization dictionary per language
        private Dictionary<string, string> PHRASES = new Dictionary<string, string>
        {
            { "AreYouSure", "Are you sure you want to exit?" },
            { "RetriesExceeded", "You have entered too many invalid commands, the program will be exited." },
            { "Retry", "That command was not recognized. Please try again." },
            { "RetryCountdown", "{0} retries remaining..." },
            { "Welcome", "Welcome to Elevator Action. What would you like to do today?" }
        };

        internal ElevatorMaster(IUserInterface ui, IConfiguration config)
        {
            _ui = ui;
            _config = config;

            //override the retry count with the config value, if present, else leave at the default
            int.TryParse(Utils.GetConfigSetting(_config, "retryCount"), out _retryCount);
        }

        internal async Task Execute()
        {
            _ui.Display(PHRASES["Welcome"]);

            //get first command
            var input = await _ui.GetCommandAsync();

            //for as long as the user is entering commands (that aren't to exit), process them
            while (input != CommandType.Exit)
            {
                PerformCommand(input);

                //once the command has been performed, wait for the next command
                input = await _ui.GetCommandAsync();
            }

            //once no more commands are received, exit
            PerformShutdownWithConfirmation();
        }

        private async void PerformCommand(CommandType command)
        {
            switch (command)
            {
                case CommandType.TryAgain:
                    await PerformRetry();
                    break;
                case CommandType.Exit:
                    await PerformShutdownWithConfirmation();
                    break;
                default:
                    _ui.ShutDown();
                    break;
            }
        }

        #region COMMAND ACTIONS

        private async Task PerformRetry()
        {
            int retryIdx = 0;
            var cmd = CommandType.TryAgain;
            bool success = false;
            var msg = string.Empty;

            //keep going until we get a command that is recognized, or run out of retries
            while (((_retryCount == -1) || (retryIdx < _retryCount)) && !success)
            {
                //if there's a limit on retries, display how many are left
                msg = (_retryCount == -1) ? PHRASES["Retry"] : $"{PHRASES["Retry"]} {string.Format(PHRASES["RetryCountdown"], retryIdx)}";
                _ui.Display(msg);

                //get the new command input
                cmd = await _ui.GetCommandAsync();

                //if the new input is valid, we can exit the while loop
                success = (cmd != CommandType.TryAgain);
            }

            if (success)
                PerformCommand(cmd);
            else
            {
                if (_retryCount > -1)
                {
                    _ui.Display(PHRASES["RetriesExceeded"]);
                    _ui.ShutDown();
                }
                // else: we should never reach this piece of code because either there's no retry limit and we're still in the while, or there was a successful command which was executed in the outer if
            }
        }

        private async Task PerformShutdownWithConfirmation()
        {
            _ui.Display(PHRASES["AreYouSure"], true);
            var confirmationResponse = await _ui.GetInputAsync();

            if (affirmativeInputs.Contains(confirmationResponse))
                _ui.ShutDown();
        }
    }
}