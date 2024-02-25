using ElevatorAction.Models;
using ElevatorAction.UserInterface;
using ElevatorTests.MockObjects;

namespace ElevatorTests.Tests
{
    [TestClass]
    public class ConsoleInterfaceTests
    {
        private string input = null;
        private TestLogger _log = new TestLogger();
        private ConsoleInterface ui;

        public ConsoleInterfaceTests()
        {
            ui = new ConsoleInterface(_log);
        }

        #region EXIT COMMAND TESTS

        [TestMethod]
        public void GetCommand_ReturnsExitOn_null()
        {
            input = null;
            var response = ui.getCommand(input);
            TestUtils.assertInputCommandMapHelper(CommandType.Exit, response, input);
        }

        [TestMethod]
        public void GetCommand_ReturnsExitOn_exit_quit()
        {
            input = "exit";
            var response = ui.getCommand(input);
            TestUtils.assertInputCommandMapHelper(CommandType.Exit, response, input);

            input = "quit";
            response = ui.getCommand(input);
            TestUtils.assertInputCommandMapHelper(CommandType.Exit, response, input);
        }

        [TestMethod]
        public void GetCommand_ReturnsExitOn_uppercaseEXIT()
        {
            input = "EXIT";
            var response = ui.getCommand(input);
            TestUtils.assertInputCommandMapHelper(CommandType.Exit, response, input);
        }

        [TestMethod]
        public void GetCommand_ReturnsTestOn_validInput()
        {
            input = "TEST";
            var response = ui.getCommand(input);
            TestUtils.assertInputCommandMapHelper(CommandType.Test, response, input);
        }

        [TestMethod]
        public void GetCommand_ReturnRetryOn_invalidInput()
        {
            input = "lorem ipsum";
            var response = ui.getCommand(input);
            TestUtils.assertInputCommandMapHelper(CommandType.TryAgain, response, input);
        }
        #endregion EXIT COMMAND TESTS
    }
}