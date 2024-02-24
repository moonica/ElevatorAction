using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElevatorAction;
using ElevatorAction.Models;
using ElevatorTests.MockObjects;

namespace ElevatorTests
{
    internal static class TestUtils
    {
        public static int nrRetries = 3;
        public static string strTested = "Tested";

        /// <summary>
        /// Assert that the given text input maps to the expected command
        /// </summary>
        /// <param name="expectedCommand"></param>
        /// <param name="actualCommand"></param>
        /// <param name="inputText"></param>
        public static void assertInputCommandMapHelper(CommandType expectedCommand, CommandType actualCommand, string inputText)
        {
            string errMsg = $"The return command for input '{inputText}' should be {expectedCommand}. {actualCommand} received instead.";

            Assert.AreEqual(expectedCommand, actualCommand, errMsg);
        }

        /// <summary>
        /// Assert that the command received matches the expected command
        /// </summary>
        /// <param name="expectedCommand"></param>
        /// <param name="actualCommand"></param>
        public static void assertCommandsMatchHelper(CommandType expectedCommand, CommandType actualCommand)
        {
            string errMsg = $"Command {expectedCommand} expected; {actualCommand} received instead.";

            Assert.AreEqual(expectedCommand, actualCommand, errMsg);
        }

        /// <summary>
        /// Assert that the output (string) received matches the expected output
        /// </summary>
        /// <param name="expectedOutput"></param>
        /// <param name="actualOutput"></param>
        public static void assertOutputsMatchHelper(string expectedOutput, string actualOutput)
        {
            string errMsg = $"Command {expectedOutput} expected; {actualOutput} received instead.";

            Assert.AreEqual(expectedOutput, actualOutput, errMsg);
        }

        /// <summary>
        /// Assert that a received output is a partial match to the expected output, by comparing the first N characters of both strings. 
        /// </summary>
        /// <param name="commandIndex"></param>
        /// <param name="expectedString"></param>
        /// <param name="actualString"></param>
        /// <param name="command"></param>
        /// <param name="nrCharsToMatch">Optional - defaults to 50 if not specified</param>
        public static void assertOutputsPartialMatchHelper(int commandIndex, string expectedString, string actualString, CommandType command, int nrCharsToMatch = 50)
        {
            var err = "Command number {0} should have been {1}. Instead the output was '{2}'. ";

            if (string.IsNullOrEmpty(expectedString))
                Assert.Fail("Could not compare outputs; expected string not provided");
            else
            {
                if (string.IsNullOrEmpty(actualString))
                    Assert.Fail("Could not compare outputs; actual string not provided");
                else
                {
                    var expectedFragment = ((expectedString?.Length ?? 0) < nrCharsToMatch) ? expectedString : expectedString?.Substring(0, nrCharsToMatch);
                    var actualFragment = ((actualString?.Length ?? 0) < nrCharsToMatch) ? actualString : actualString?.Substring(0, nrCharsToMatch);

                    Assert.AreEqual(
                        expectedFragment, actualFragment,
                        string.Format(err, commandIndex, command.ToString(), actualString)
                    );
                }
            }
        }

        public static void assertExitOutputsHelper<TLambdaInput>(TestInterface ui, Action<TLambdaInput> performCommand, TLambdaInput commandInput, string confirmationInputToTest, string expectedConfirmationOutput, bool shouldExit = true)
        {
            ui.TestInput = confirmationInputToTest;

            performCommand(commandInput);

            if ((ui.outputs?.Count ?? 0) < (shouldExit ? 2 : 1))
            {
                Assert.Fail($"Input: {confirmationInputToTest}. Too few outputs detected from ElevatorMaster. 2 expected, {ui.outputs?.Count} outputs were received");
            }
            else
            {
                //the first output should be the confirmation message
                Assert.AreEqual(expectedConfirmationOutput, ui.outputs[0], $"Expected confirmation to exit. Received: {ui.outputs[0]}");

                if (shouldExit)
                    //the last output should be exit/abort message
                    Assert.AreEqual(TestInterface.ExitString, ui.outputs[1], $"Expected termination of program. Received: {ui.outputs[1]}");
            }
        }
    }
}
