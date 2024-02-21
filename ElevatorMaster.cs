using ElevatorAction.UserInterface;
using ElevatorAction.Models;

namespace ElevatorAction
{
    internal class ElevatorMaster
    {
        private IUserInterface _ui;

        private readonly List<string> affirmativeInputs = new List<string> { "YES", "Y" };

        //All natural language phrases are enumerated here for ease of change; they could also be put in a localization dictionary per language
        private Dictionary<string, string> PHRASES = new Dictionary<string, string>
        {
            { "AreYouSure", "Are you sure you want to exit?" },
            { "Welcome", "Welcome to Elevator Action. What would you like to do today?" }
        };

        internal ElevatorMaster(IUserInterface ui)
        {
            _ui = ui;
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
            ShutdownWithConfirmation();
        }

        private async void PerformCommand(CommandType command)
        {
            switch (command)
            {
                //case ConsoleCommand.TryAgain:
                //    break;
                case CommandType.Exit:
                    await ShutdownWithConfirmation();
                    break;
                default:
                    _ui.ShutDown();
                    break;
            }
        }

        private async Task ShutdownWithConfirmation()
        {
            _ui.Display(PHRASES["AreYouSure"], true);
            var confirmationResponse = await _ui.GetInputAsync();

            if (affirmativeInputs.Contains(confirmationResponse))
                _ui.ShutDown();
        }
    }
}