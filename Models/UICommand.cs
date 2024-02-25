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
    public class UICommand
    {
        //User friendly descriptions for all elevator console commands. They could also be put in a localization dictionary per language. Private/control commands do not need to be included here as they shouldn't appear in the Help documentation
        public static Dictionary<CommandType, string> CommandDescriptions = new Dictionary<CommandType, string>()
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

        /// <summary>
        /// The format a list of commands should be displayed in
        /// </summary>
        public readonly static string HelpFormat = "[{0}] {1} : {2}";

        /// <summary>
        /// The type of command this instance is
        /// </summary>
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
                return CommandDescriptions[ThisCommandType];
            }
        }


        /// <summary>
        /// Return a list of all possible commands, with their descriptions.
        /// </summary>
        /// <param name="format">Optional. If left blank, the default format is used. If specified, it needs to contain parameters (in the format '{0}') for, in this order: command number, command name, command description</param>
        /// <returns></returns>
        public static List<string> ListPublicCommands(string format = null)
        {
            var result = new List<string>();
            string cmdStr;
            var commands = Enum.GetValues<CommandType>();
            int i = 1;

            foreach (var cmd in commands)
            {
                //Only show public commands
                if (isCommandPublic.ContainsKey(cmd) && !isCommandPublic[cmd])
                    continue;

                cmdStr = string.Format((format ?? HelpFormat), i++, cmd.ToString(),
                    (CommandDescriptions.ContainsKey(cmd)
                    ? CommandDescriptions[cmd]
                    : ""));


                result.Add(cmdStr);
            }

            return result;
        }

        /// <summary>
        /// Use this method to easily see how many commands there are that are public/accessible to the end user
        /// </summary>
        /// <returns></returns>
        public static int? CountPublicCommands()
        {
            return isCommandPublic.Where(kv => kv.Value == true)?.Count();
        }
    }
}
