using ElevatorAction.UserInterface;
using ElevatorAction.Models;
using Microsoft.Extensions.Configuration;

namespace ElevatorAction
{
    internal class Program
    {
        /*
          The use of dependency injection for the user interface is a little contrived here, but as much as possible without over-engineering past the point of the requirements given (a console application per se), I have attempted to decouple the entry point from the console specific interface details. It would need some additional work to truly become loosely coupled but I have attempted to minimise that technical debt, and to illustrate how dependency injection can be generally used for seperation of concerns and clean architecture.
          -Jonica Brown Feb 2024
        */

        private static readonly IConfiguration Configuration;

        static void Main(string[] args)
        {
            var elevatorMaster = new ElevatorMaster(new ConsoleInterface(), Configuration, Utils.Phrases_en);
            _ = elevatorMaster.Execute(); //console applications can't really be async, so we can't await the async Execute method
        }
    }
}