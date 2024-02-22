using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElevatorAction.UserInterface;

namespace ElevatorTests
{
    internal class TestInterface : IUserInterface
    {
        public CommandType TestCommand = CommandType.Exit;
        public string TestInput = string.Empty;

        public void Display(string message, bool isConfirmation = false)
        {
            Debug.WriteLine(message);
        }

        public async Task<CommandType> GetCommandAsync()
        {
            return TestCommand;
        }

        public async Task<string> GetInputAsync()
        {
            return TestInput;
        }

        public void ShutDown()
        {
            Debug.WriteLine("Application ended");
        }
    }
}
