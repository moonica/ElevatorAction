using ElevatorAction.UserInterface;
using ElevatorAction.Models;
using Microsoft.Extensions.Configuration;

namespace ElevatorAction
{
    internal class ElevatorController
    {
        private IUserInterface _ui;
        private int _retryCount = 5; //default

        private readonly List<string> affirmativeInputs = new List<string> { "YES", "Y" };

        internal ElevatorController(IUserInterface ui, Dictionary<string, string> phrases, int? retryCount = null)
        {
            _ui = ui;

            if (validateRetryCount(retryCount))
                _retryCount = retryCount.Value;
        }

        private bool validateRetryCount(int? retryCount)
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
            }

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

            if (affirmativeInputs.Contains(confirmationResponse))
                _ui.ShutDown();
        }

        public void PerformShutdownWithFinalMessage(string message)
        {
            _ui.Display(message);
            _ui.ShutDown();
        }
    }
}