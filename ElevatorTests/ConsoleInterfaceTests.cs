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
        public void GetCommandReturnsExitOn_null()
        {
            input = null;
            var response = ui.getCommand(input);
            TestUtils.assertCommandHelper(input, CommandType.Exit, response);
        }

        [TestMethod]
        public void GetCommandReturnsExitOn_exit_quit()
        {
            input = "exit";
            var response = ui.getCommand(input);
            TestUtils.assertCommandHelper(input, CommandType.Exit, response);

            input = "quit";
            response = ui.getCommand(input);
            TestUtils.assertCommandHelper(input, CommandType.Exit, response);
        }

        [TestMethod]
        public void GetCommandReturnsExitOn_uppercaseEXIT()
        {
            input = "EXIT";
            var response = ui.getCommand(input);
            TestUtils.assertCommandHelper(input, CommandType.Exit, response);
        }

        #endregion EXIT COMMAND TESTS
    }
}