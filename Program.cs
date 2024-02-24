using ElevatorAction.UserInterface;
using ElevatorAction.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ElevatorAction
{
    internal class Program
    {
        private static IConfiguration _configuration;
        private static ILogger _logger;

        static void Main(string[] args)
        {
            var elevatorMaster = new ElevatorMaster(new ConsoleInterface(), _configuration, _logger, Utils.Phrases_en);

            //console applications can't really be async, so we can't await the async Execute method
            _ = elevatorMaster.Execute(); 
        }
    }
}