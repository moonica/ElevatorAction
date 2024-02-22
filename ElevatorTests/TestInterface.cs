using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElevatorAction.UserInterface;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

namespace ElevatorTests
{
    internal class TestInterface : IUserInterface
    {
        private List<CommandType> listOfCommands;
        private int commandListCounter = 0;
        
        public CommandType TestCommand = CommandType.Exit;
        public string TestInput = string.Empty;
        public bool multipleCommands = false;
        public List<string> outputs = new List<string>();
        public string ExitString = "Application ended";

        public TestInterface() 
        {
            Reset();
        }

        public void Display(string message, bool isConfirmation = false)
        {
            Debug.WriteLine(message);
            outputs.Add(message);
        }

        public async Task<CommandType> GetCommandAsync()
        {
            if (!multipleCommands)
                return TestCommand;
            else
            {
                //if we need to test something that returns different commands in a sequence, return the current one and move on the counter
                if (commandListCounter < listOfCommands.Count)
                    return listOfCommands[commandListCounter++];
                else
                    return TestCommand;
            }
        }

        public async Task<string> GetInputAsync()
        {
            return TestInput;
        }

        public void ShutDown()
        {
            Debug.WriteLine(ExitString);
            outputs.Add(CommandType.Exit.ToString());
        }

        public void SetMultipleCommands(List<CommandType> commands)
        {
            multipleCommands = true;
            commandListCounter = 0;

            listOfCommands = commands;
        }

        public void Reset()
        {
            multipleCommands = false;
            commandListCounter = 0;
            listOfCommands = new List<CommandType>();
            outputs= new List<string>();
        }
    }
}
