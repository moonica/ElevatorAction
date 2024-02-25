using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElevatorAction;
using ElevatorAction.Models;
using ElevatorAction.UserInterface;
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
        public static void assertCommandsMatchHelper(CommandType expectedCommand, CommandType actualCommand, int? commandIndex = null)
        {
            var errMsg = new StringBuilder()
                .Append(commandIndex.HasValue ? $"index={commandIndex.Value} | " : "")
                .Append($"Command {expectedCommand} expected; {actualCommand} received instead.");

            Assert.AreEqual(expectedCommand, actualCommand, errMsg.ToString());
        }

        /// <summary>
        /// Assert that the output (string) received matches the expected output
        /// </summary>
        /// <param name="expectedOutput"></param>
        /// <param name="actualOutput"></param>
        public static void assertOutputsMatchHelper(string expectedOutput, string actualOutput, int? commandIndex = null)
        {
            var errMsg = new StringBuilder()
                .Append(commandIndex.HasValue ? $"index={commandIndex.Value} | " : "")
                .Append($"Output '{expectedOutput}' expected; '{actualOutput}' received instead.");

            Assert.AreEqual(expectedOutput, actualOutput, errMsg.ToString());
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

        /// <summary>
        /// Assert that, when exiting/aborting, the optional confirmation is displayed if required, and the next command is the exit/abort by checking the outputs
        /// </summary>
        /// <typeparam name="TLambdaInput"></typeparam>
        /// <param name="ui"></param>
        /// <param name="performCommand"></param>
        /// <param name="commandInput"></param>
        /// <param name="confirmationInputToTest"></param>
        /// <param name="expectedConfirmationOutput"></param>
        /// <param name="shouldExit"></param>
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

        /// <summary>
        /// We must test that the controller displays the help text when that method is called. We must also test that the ElevatorMaster switches correctly on the "HELP" command call that method on the controller. This function is reusable and tests the output for both scenarios (since there isn't another way to test eg. just the switching)
        /// </summary>
        public static void assertHelpCommandsDisplayed(Action performCommand, TestInterface ui)
        {
            var publicCommandCount = UICommand.CountPublicCommands();
            int charsToTestForFormat = 4;

            if (publicCommandCount is null)
            {
                Assert.IsTrue(1 == 1, "No public commands found; so we can't really define success for this test");
                return;
            }

            performCommand();

            //check the right number of items were displayed
            Assert.IsTrue(ui.outputs?.Count() == publicCommandCount);

            if (ui.outputs == null)
            {
                Assert.Fail("No ui outputs received");
                return;
            }

            string expectedStr, resultStr;

            //check a sample of outputs for the right format
            for (int i = 0; i < Math.Min(3, (ui.outputs?.Count ?? 0)); i++)
            {
                //we are just looking for the first few characters to check the number is present and in the right format
                expectedStr = string.Format(UICommand.HelpFormat, i+1, string.Empty, string.Empty).Substring(0, charsToTestForFormat);
                resultStr = ui.outputs[i].Substring(0, charsToTestForFormat);

                Assert.IsTrue(expectedStr.Equals(resultStr), $"Expected first {charsToTestForFormat} characters of output number {i}: '{expectedStr}'. Received instead: '{resultStr}'");
            }

            //check the first entry for the right description
            var firstPublicCommand = UICommand.isCommandPublic.First(kv => kv.Value == true).Key;
            expectedStr = string.Format(
                UICommand.HelpFormat,
                1,
                firstPublicCommand.ToString(),
                UICommand.CommandDescriptions[firstPublicCommand]);

            Assert.IsTrue(expectedStr.Equals(ui.outputs.FirstOrDefault()), $"Expected first output not received. Expected: '{expectedStr}'; received: '{ui.outputs.FirstOrDefault()}'");
        }
    }
}
