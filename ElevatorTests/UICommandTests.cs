using ElevatorAction.Models;
using System.Data;

namespace ElevatorTests
{
    [TestClass]
    public class UICommandTests
    {
        [TestMethod]
        public void ListPublicCommands_ReturnsAllPublicCommands()
        {
            var result = UICommand.ListPublicCommands();
            var resultCount = result?.Count();
            var publicCount = UICommand.isCommandPublic.Where(kv => kv.Value == true)?.Count() ?? 0;

            Assert.AreEqual(publicCount, resultCount, $"Expected {publicCount} public commands, received {resultCount}");

            //check no returned command is private
            var privateCommands = UICommand.isCommandPublic.Where(kv => kv.Value == false)?.Select(kv => kv.Key).ToList();

            var matchingDescriptions = new List<ElevatorAction.Models.CommandType>();
            var matchingPublicFlags = new List<KeyValuePair<ElevatorAction.Models.CommandType, bool>>();

            //check no descriptions match private keys
            foreach (var desc in result)
            {
                matchingDescriptions = UICommand.commandDescriptions
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
