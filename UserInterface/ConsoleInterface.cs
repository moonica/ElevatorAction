using ElevatorAction.Models;
using Microsoft.Extensions.Logging;
using System.Reflection.Metadata.Ecma335;
using static System.Net.Mime.MediaTypeNames;

namespace ElevatorAction.UserInterface
{
    public class ConsoleInterface : IUserInterface
    {
        #region PRIVATE PROPERTIES

        private ILogger _logger;

        //This dictionary makes the list of set commands easily (and cheaply) retrieved by string keyword, but minimizes the need for error prone and unreadable text matching throughout the code
        private readonly Dictionary<string, CommandType> commandList = new Dictionary<string, CommandType> {
            { string.Empty, CommandType.TryAgain },
            { "ABORT", CommandType.Abort },
            { "CALLELEVATOR", CommandType.CallElevator },
            { "CREATEELEVATOR", CommandType.CreateElevator },
            { "DELETEELEVATOR", CommandType.DeleteElevator },
            { "DISABLEELEVATOR", CommandType.DisableElevator },
            { "ENABLEELEVATOR", CommandType.EnableElevator },
            { "EXIT", CommandType.Exit },
            { "HELP", CommandType.Help },
            { "DETAILS", CommandType.ListElevatorDetails },
            { "LIST", CommandType.ListElevators },
            { "STATES", CommandType.ListElevatorStates },
            { "TRYAGAIN", CommandType.TryAgain },
            { "TEST", CommandType.Test },
            { "QUIT", CommandType.Exit }
        };

        private readonly string CONFIRM_STRING = "Type 'Y' to confirm or 'N' to cancel";

        #endregion PRIVATE PROPERTIES


        #region CONSTRUCTORS
        
        public ConsoleInterface(ILogger logger)
        {
            _logger = logger;
        }
        
        #endregion CONSTRUCTORS



        #region PUBLIC PROPERTIES
        public string PressAnyKeyTranslationKey
        {
            get
            {
                return "PressAnyKey";
            }
        }

        public ILogger UILogger
        {
            get
            {
                return _logger;
            }
            set
            {
                _logger = value;
            }
        }

        #endregion PUBLIC PROPERTIES


        #region PUBLIC METHODS

        //Not private due to testing visibility
        public CommandType getCommand(string input)
        {
            //if no valid input was received, exit
            if (input is null)
            {
                return CommandType.Exit;
            }

            //the input wasn't recognized
            if (!commandList.ContainsKey(input.Trim().ToUpper()))
            {
                return CommandType.TryAgain;
            }

            //the input was one of the set commands
            return commandList[input.Trim().ToUpper()];
        }

        public void Display(string message, bool isConfirmation = false)
        {
            if (isConfirmation)
                //Rather than use Console.Read to read in a single letter "Y" or "N", keep the input model consistent for ease of use. All commands must be followed by ENTER.
                Console.WriteLine($"{message} {CONFIRM_STRING}");
            else
                Console.WriteLine(message);
        }

        public void Display(List<string> messages)
        {
            if ((messages?.Count() ?? 0) == 0)
            {
                _logger.LogWarning("Method Display(List<string>) received empty/null message list; nothing displayed");
                return;
            }

            foreach (var msg in messages)
                Console.WriteLine(msg);
        }

        public async Task<string> GetInputAsync()
        {
            //read input from the command line
            return Console.ReadLine()?.Trim()?.ToUpper() ?? null;
        }

        public async Task<CommandType> GetCommandAsync()
        {
            var input = await GetInputAsync();

            return getCommand(input);
        }
        
        public void ShutDown()
        {
            Environment.Exit(0);
        }

        public Task WaitForUserActionAsync()
        {
            Console.Read();
            return Task.CompletedTask;
        }

        #endregion PUBLIC METHODS
    }
}