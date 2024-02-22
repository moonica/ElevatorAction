using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElevatorAction.UserInterface;


namespace ElevatorTests
{
    internal static class TestUtils
    {
        private static Dictionary<string, string> errorMessages = new Dictionary<string, string>
        {
            { "shouldBeExit", "The return command for input {0} should be 'Exit'. {1} received instead." }
        };

        public static void assertCommandHelper(string input, CommandType commandTargetType)
        {
            var response = ui.getCommand(input);

            Assert.AreEqual(commandTargetType, response, string.Format(errorMessages["shouldBeExit"], "'null'", response.ToString()));
        }
    }
}
