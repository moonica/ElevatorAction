using ElevatorAction.Models;
using ElevatorAction.UserInterface;

namespace ElevatorTests
{
    [TestClass]
    public class ConsoleInterfaceTests
    {
        private string input = null;
        private ConsoleInterface ui = new ConsoleInterface();

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

        #endregion EXIT COMMAND TESTS
    }
}