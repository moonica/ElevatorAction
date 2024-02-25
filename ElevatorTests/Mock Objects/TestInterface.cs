using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElevatorAction.Models;
using ElevatorAction.UserInterface;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

namespace ElevatorTests.MockObjects
{
    internal class TestInterface : IUserInterface
    {
        private List<CommandType> listOfCommands;
        private List<string> listOfInputs;
        private int commandListCounter = 0;
        private ILogger _logger;
        private int listCounter = 0;
        
        public CommandType TestCommand = CommandType.Exit;
        public string TestInput;
        public bool multipleCommands = false;
        public bool multipleInputs = false;
        public List<string> outputs = new List<string>();
        public static string ExitString = "Application ended";

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

        public TestInterface(ILogger logger) 
        {
            _logger = logger;
            Reset();
        }

        public void Display(string message, bool isConfirmation = false)
        {
            //no point in mimicing a confirmation Y/N input so ignore the second param

            Debug.WriteLine(message);
            outputs.Add(message);
        }

        public void Display(List<string> messages)
        {
            foreach (var msg in messages)
            {
                Display(msg);
            }
        }

        public async Task<CommandType> GetCommandAsync()
        {
            if (!multipleCommands)
                return TestCommand;
            else
            {
                //if we need to test something that returns different commands in a sequence, return the current one and move on the counter
                if (listCounter < listOfCommands.Count)
                    return listOfCommands[listCounter++];
                else
                    return TestCommand;
            }
        }

        public async Task<string> GetInputAsync()
        {
            if (!multipleInputs)
                return TestInput;
            else
            {
                //if we need to test something that returns different inputs in a sequence, return the current one and move on the counter
                if (listCounter < listOfInputs.Count)
                    return listOfInputs[listCounter++];
                else
                    return TestInput;
            }
        }

        public void ShutDown()
        {
            Debug.WriteLine(ExitString);
            outputs.Add(ExitString);
        }

        /// <summary>
        /// Use this method to set a sequence of commands, to immitate a user entering subsequent commands 
        /// </summary>
        /// <param name="commands"></param>
        public void SetMultipleCommands(List<CommandType> commands)
        {
            multipleCommands = true;
            listCounter = 0;

            listOfCommands = commands;
        }

        /// <summary>
        /// Use this method to set a sequence of text inputs, to immitate a user entering subsequent inputs 
        /// </summary>
        /// <param name="inputs"></param>
        public void SetMultipleInputs(List<string> inputs)
        {
            multipleInputs = true;
            listCounter = 0;

            listOfInputs = inputs;
        }

        public void Reset()
        {
            multipleCommands = false;
            multipleInputs = false;
            listCounter = 0;
            listOfCommands = new List<CommandType>();
            listOfInputs = new List<string>();
            outputs = new List<string>();
            TestInput = null;
        }

        public Task WaitForUserActionAsync()
        {
            return Task.CompletedTask;
        }
    }
}
