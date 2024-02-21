namespace ElevatorAction.UserInterface
{
    public interface IUserInterface
    {
        #region PUBLIC/INTERNAL METHODS        

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

        public void ShutDown();

        #endregion PUBLIC/INTERNAL METHODS
    }
}