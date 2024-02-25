using ElevatorAction.UserInterface;
using ElevatorAction.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ElevatorAction
{
    public class ElevatorController
    {
        private IUserInterface _ui;
        private ILogger _log;
        private int _retryCount = 5; //default

        private readonly List<string> affirmativeInputs = new List<string> { "YES", "Y" };

        public ElevatorController(IUserInterface ui, ILogger log, int? retryCount = null)
        {
            _ui = ui;
            _log = log;

            //usually the retrycount comes from the config, 
            if (retryCount.HasValue)
                _retryCount= retryCount.Value;
        }

        public bool validateRetryCount(int? retryCount)
        {
            return 
                (
                    retryCount.HasValue &&
                    retryCount == -1
                ) || (
                    retryCount.HasValue &&
                    retryCount > 0 &&
                    retryCount < int.MaxValue
                );
        }

        public async Task<CommandType> GetCommandAfterRetry(string retryMessage, string retryCountdownMessage)
        {
            int retryIdx = 0;
            var cmd = CommandType.TryAgain;
            bool success = false;
            var msg = string.Empty;

            if (!validateRetryCount(_retryCount))
            {
                var errMsg = $"The configuration value for max number of retries is invalid";

                _log.LogError($"{errMsg} : {_retryCount}");

                throw new ArgumentException($"{errMsg}. Check that it is either a valid, positive integer or -1 to indicate infinate retries.");
            }

            //keep going until we get a command that is recognized, or run out of retries
            while (((_retryCount == -1) || (retryIdx < _retryCount)) && !success)
            {

                //if there's a limit on retries, display how many are left
                msg = (_retryCount == -1) ? retryMessage : $"{retryMessage} {string.Format(retryCountdownMessage, _retryCount - retryIdx - 1)}";
                _ui.Display(msg);

                //get the new command input
                cmd = await _ui.GetCommandAsync();

                //if the new input is valid, we can exit the while loop
                success = (cmd != CommandType.TryAgain);
                retryIdx++;
            }

            _log.LogInformation($"[ElevatorController.GetCommandAfterRetry] Internal while loop ran {retryIdx} times before valid command {cmd} was entered");

            //we exited the while loop; let's findout why
            if (success)
                //we got a usable command. Let's go execute it
                return cmd;
            else
            {
                if (_retryCount > -1)
                    //no usable command and we reached max retries
                    return CommandType.Abort;
                else
                {
                    // we should never reach this piece of code because either there's no retry limit and we're still in the while, the limit was reached and we should exit, or there was a successful command which was executed in the outer if. But all code paths must return a value
                    return CommandType.Abort;
                }
            }
        }

        public async Task PerformShutdownWithConfirmation(string confirmationMessage)
        {
            _ui.Display(confirmationMessage, true);
            var confirmationResponse = await _ui.GetInputAsync();

            if (affirmativeInputs.Contains(confirmationResponse?.Trim()?.ToUpper()))
                _ui.ShutDown();
        }

        public async Task PerformShutdownWithFinalMessage(string message, bool waitForUserAction = false)
        {
            _ui.Display(message);

            if (waitForUserAction)
                await _ui.WaitForUserActionAsync();

            _ui.ShutDown();
        }
    }
}