using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorAction.Models
{
    /// <summary>
    /// An object that represents a command together with its details
    /// </summary>
    internal class UICommand
    {
        //User friendly descriptions for all elevator console commands. They could also be put in a localization dictionary per language. Private/control commands do not need to be included here as they shouldn't appear in the Help documentation
        private static Dictionary<CommandType, string> commandDescriptions = new Dictionary<CommandType, string>()
        {
            { CommandType.CallElevator, "A user can use this command to call an elevator to their current floor" },
            { CommandType.CreateElevator, "Use this command to add a new elevator to the database" },
            { CommandType.DeleteElevator, "Use this command to remove an elevator permanently from the database" },
            { CommandType.DisableElevator, "Use this command to remove an elevator from current service (without removing it from the database)" },
            { CommandType.EnableElevator, "Use this command to re-enable a disabled elevator for use (assuming it exists in the database)" },
            { CommandType.Exit, "Exit/close the application" },
            { CommandType.Help, "See a list of all commands you can use with this application from the command line" },
            { CommandType.ListElevatorDetails, "See a detailed list of all elevators with their information such as description and serial number" },
            { CommandType.ListElevators, "See a summary list of all elevators currently in operation" },
            { CommandType.ListElevatorStates, "See a list of all elevators currently in operation with their states (eg. current floor, nr passengers, movement)" }
        };

        //public for testing purposes
        public static Dictionary<CommandType, bool> isCommandPublic = new Dictionary<CommandType, bool>()
        {
            { CommandType.Abort, false },
            { CommandType.CallElevator, true },
            { CommandType.CreateElevator, true },
            { CommandType.DeleteElevator, true },
            { CommandType.DisableElevator, true },
            { CommandType.EnableElevator, true },
            { CommandType.Exit, true },
            { CommandType.Help, true },
            { CommandType.ListElevatorDetails, true },
            { CommandType.ListElevators, true },
            { CommandType.ListElevatorStates, true },
            { CommandType.TryAgain, false },
            { CommandType.Test, false }
        };

        public CommandType ThisCommandType;

        /// <summary>
        /// Readonly. String representation of the command name
        /// </summary>
        public string CommandName
        {
            get
            {
                return ThisCommandType.ToString()?.Trim()?.ToUpper();
            }
        }

        public string CommandDescription
        {
            get
            {
                return commandDescriptions[ThisCommandType];
            }
        }


        /// <summary>
        /// Return a list of all possible commands, with their descriptions.
        /// </summary>
        /// <returns>Returns a list of commands with descriptions</returns>
        public static List<string> ListPublicCommands()
        {
            var result = new List<string>();
            var strBuilder = new StringBuilder();
            var commands = Enum.GetValues<CommandType>();
            int i = 0;

            foreach (var cmd in commands)
            {
                //Only show public commands
                if (isCommandPublic.ContainsKey(cmd) && !isCommandPublic[cmd])
                    continue;

                strBuilder = new StringBuilder("[").Append(i).Append("] ")
                    .Append(cmd.ToString()).Append(": ");                

                if (commandDescriptions.ContainsKey(cmd))
                    strBuilder.Append(commandDescriptions[cmd]);

                //do not add line breaks, leave that for the interface to figure out for its specific format

                result.Add(strBuilder.ToString());
                i++;
            }

            return result;
        }
    }
}
