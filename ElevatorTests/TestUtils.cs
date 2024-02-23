﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElevatorAction;
using ElevatorAction.UserInterface;
using ElevatorTests.MockObjects;

namespace ElevatorTests
{
    internal static class TestUtils
    {
        public static int nrRetries = 3;

        public static void assertCommandHelper(string inputText, CommandType expectedCommand, CommandType actualCommand)
        {
            var errMsg = "The return command for input '{0}' should be {1}. {2} received instead.";

            Assert.AreEqual(expectedCommand, actualCommand, string.Format(errMsg, inputText, expectedCommand.ToString(), actualCommand.ToString()));
        }

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
                    var expectedFragment = ((expectedString?.Length ?? 0) < nrCharsToMatch) ? expectedString : expectedString.Substring(0, nrCharsToMatch);
                    var actualFragment = ((actualString?.Length ?? 0) < nrCharsToMatch) ? actualString : actualString.Substring(0, nrCharsToMatch);

                    Assert.AreEqual(
                        expectedFragment, actualFragment,
                        string.Format(err, commandIndex, command.ToString(), actualString)
                    );
                }
            }
        }

        public static void assertExitOutputsHelper(TestInterface ui, Action<CommandType> performCommand, string confirmationInputToTest, string expectedConfirmationOutput, bool shouldExit = true)
        {
            ui.TestInput = confirmationInputToTest;

            performCommand(CommandType.Exit);

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
