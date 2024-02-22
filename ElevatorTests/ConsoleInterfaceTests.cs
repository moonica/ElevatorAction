using ElevatorAction.UserInterface;

namespace ElevatorTests
{
    [TestClass]
    public class ConsoleInterfaceTests
    {
        private string input = null;
        private ConsoleInterface ui = new ConsoleInterface();

        private Dictionary<string, string> errorMessages = new Dictionary<string, string>
        {
            { "shouldBeExit", "The return command for input {0} should be 'Exit'. {1} received instead." }
        };

        private void assertCommandHelper(string input, CommandType commandTargetType)
        {
            var response = ui.getCommand(input);

            Assert.AreEqual(commandTargetType, response, string.Format(errorMessages["shouldBeExit"], "'null'", response.ToString()));
        }

        [TestMethod]
        public void getCommandReturnsExitOn_null()
        {
            assertCommandHelper(null, CommandType.Exit);
        }

        [TestMethod]
        public void getCommandReturnsExitOn_exit_quit()
        {
            assertCommandHelper("exit", CommandType.Exit);
            assertCommandHelper("quit", CommandType.Exit);
        }

        [TestMethod]
        public void getCommandReturnsExitOn_uppercase()
        {
            assertCommandHelper("EXIT", CommandType.Exit);
        }
    }
}