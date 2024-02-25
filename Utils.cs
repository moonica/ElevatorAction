using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorAction
{
    public static class Utils
    {
        //All natural language phrases are enumerated here for ease of change; they could also be put in a localization dictionary per language
        public static Dictionary<string, string> Phrases_en = new Dictionary<string, string>
        {
            { "AreYouSure", "Are you sure you want to exit?" },
            { "Goodbye", "Goodbye" },
            { "PressAnyKey", "Press any key to continue" },
            { "RetriesExceeded", "You have entered too many invalid commands, the program will be exited." },
            { "Retry", "That command was not recognized. Please try again, or enter 'Help' to see a list of valid commands." },
            { "RetryCountdown", "{0} retries remaining..." },
            { "Welcome", "Welcome to Elevator Action. What would you like to do today? Type 'help' to see a list of commands." }
        };

        public static string GetConfigSetting(IConfiguration config, string keyName)
        {
            if (!string.IsNullOrEmpty(keyName))
                return config[keyName];
            else
                return null;
        }
    }
}
