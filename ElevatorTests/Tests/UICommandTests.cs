using ElevatorAction.Models;
using System.Data;

namespace ElevatorTests.Tests
{
    [TestClass]
    public class UICommandTests
    {
        [TestMethod]
        public void CountPublicCommands_returnsCorrectNumber()
        {
            //don't use linq as that's all the method does, let's make sure the linq is constructed correctly by counting manually
            int publicCommands = 0;

            foreach (var thisCommand in UICommand.isCommandPublic)
            {
                //Adding "== true" is unnecessary but improves readability
                if (thisCommand.Value == true)
                    publicCommands++;
            }

            if (UICommand.CountPublicCommands() is null)
            {
                Assert.Fail("CountPublicCommands returned no data (is null)");
                return;
            }

            var result = UICommand.CountPublicCommands().Value;
            Assert.IsTrue(publicCommands == result, $"Expected {publicCommands} public commands; received {result}");
        }

        [TestMethod]
        public void ListPublicCommands_ReturnsAllPublicCommands()
        {
            var result = UICommand.ListPublicCommands();
            var resultCount = result?.Count();
            var publicCount = UICommand.CountPublicCommands();

            Assert.AreEqual(publicCount, resultCount, $"Expected {publicCount} public commands, received {resultCount}");

            //check no returned command is private
            var privateCommands = UICommand.isCommandPublic.Where(kv => kv.Value == false)?.Select(kv => kv.Key).ToList();

            var matchingDescriptions = new List<ElevatorAction.Models.CommandType>();
            var matchingPublicFlags = new List<KeyValuePair<ElevatorAction.Models.CommandType, bool>>();

            //check no descriptions match private keys
            foreach (var desc in result)
            {
                matchingDescriptions = UICommand.CommandDescriptions
                    .Where(kv => kv.Value == desc)
                    .Select(kv => kv.Key)
                    .ToList();

                if (!matchingDescriptions.Any())
                    break;

                matchingPublicFlags = UICommand.isCommandPublic
                     .Where(kv => matchingDescriptions.Contains(kv.Key))
                     .ToList();

                if (!matchingPublicFlags.Any())
                    break;

                Assert.IsFalse(matchingPublicFlags.Any(kv => kv.Value == false));
            }
        }

    }
}
