using ElevatorAction.UserInterface;
using ElevatorAction.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ElevatorAction
{
    public class ElevatorMaster
    {
        private ILogger _logger;
        private IUserInterface _ui;
        private readonly IConfiguration _config;
        private readonly int _retryCount = 5; //default
        private ElevatorController controller;
        private Dictionary<string, string> _phrases = new Dictionary<string, string>();

        public ElevatorMaster(IUserInterface ui, IConfiguration config, ILogger logger, Dictionary<string, string> phrases)
        {
            try
            {
                _ui = ui;
                _config = config;
                _logger = logger;

                //Get all phrases from the utils for now, this could come from translation files later
                _phrases = phrases;

                //override the retry count with the config value, if present, else leave at the default
                int.TryParse(Utils.GetConfigSetting(_config, "retryCount"), out _retryCount);

                controller = new ElevatorController(_ui, _retryCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, null);

                _ui?.Display($"The application encountered the following unexpected error occurred and will be shut down: '{ex.Message}'");

                if (controller is null)
                    _ui?.ShutDown();
                else
                    controller?.PerformShutdownWithFinalMessage(_phrases[_ui?.PressAnyKeyTranslationKey], true);
            }
        }

        internal async Task Execute()
        {
            _ui.Display(_phrases["Welcome"]);
            _logger.Log(LogLevel.Information, $"Elevator Action program started [{DateTime.Now.ToString("yyyyMMdd HH:mm:ss")}]");

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
                case CommandType.Test:
                    //Test command used in unit tests to verify correct switching
                    _ui.Display("Tested");
                    break; 
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