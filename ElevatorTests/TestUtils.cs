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
        public static void assertCommandHelper(string inputText, CommandType expectedCommand, CommandType actualCommand)
        {
            var errMsg = "The return command for input '{0}' should be {1}. {2} received instead.";

           Assert.AreEqual(expectedCommand, actualCommand, string.Format(errMsg, inputText, expectedCommand.ToString(), actualCommand.ToString()));
        }
    }
}
