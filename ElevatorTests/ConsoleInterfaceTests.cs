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
        public void getCommandReturnsExitOn_null()
        {
            TestUtils.assertCommandHelper(null, CommandType.Exit);
        }

        [TestMethod]
        public void getCommandReturnsExitOn_exit_quit()
        {
            TestUtils.assertCommandHelper("exit", CommandType.Exit);
            TestUtils.assertCommandHelper("quit", CommandType.Exit);
        }

        [TestMethod]
        public void getCommandReturnsExitOn_uppercase()
        {
            TestUtils.assertCommandHelper("EXIT", CommandType.Exit);
        }

        #endregion EXIT COMMAND TESTS
    }
}