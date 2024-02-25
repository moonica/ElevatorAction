using ElevatorAction.Models;
using Microsoft.Extensions.Logging;

namespace ElevatorAction.UserInterface
{
    public interface IUserInterface
    {        
        #region PUBLIC PROPERTIES        

        /// <summary>
        /// This readonly property specifies which key in the translations/phrases contains the right message for this type of interface - a message to indicate the user needs to perform one more action before flow resumes. Eg, "press any key to exit". This is NOT the phrase, but the translation key.
        /// </summary>
        string PressAnyKeyTranslationKey { get; }

        /// <summary>
        /// Interfaces must be able to log errors etc
        /// </summary>
        ILogger UILogger { get; set; }

        #endregion PUBLIC PROPERTIES        


        #region PUBLIC METHODS        

        /// <summary>
        /// Asynchronously read input from the interface, if valid
        /// </summary>
        /// <returns></returns>
        public Task<string> GetInputAsync();

        /// <summary>
        /// Asynchronously read input from the interface and, if valid, return one of the defined commands in order to decide which action to perform
        /// </summary>
        /// <returns></returns>
        public Task<CommandType> GetCommandAsync();

        /// <summary>
        /// Display a string message on the interface 
        /// </summary>
        /// <param name="message"></param>
        public void Display(string message, bool isConfirmation = false);

        /// <summary>
        /// Display a list of string messages on the interface 
        /// </summary>
        /// <param name="messages"></param>
        public void Display(List<string> messages);

        /// <summary>
        /// Exit the application
        /// </summary>
        public void ShutDown();

        /// <summary>
        /// Async. Signals the calling code that a user performed an action before eg. a shutdown or a message disappearing. Equivalent of console "press any key to continue"; the input itself doesn't matter and is not passed back to the caller
        /// </summary>
        public Task WaitForUserActionAsync();

        #endregion PUBLIC METHODS
    }
}