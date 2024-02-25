using ElevatorAction.Models;
using ElevatorAction.Repository;
using Microsoft.Extensions.Logging;

namespace ElevatorAction.Repository
{
    public class ElevatorRepository<TCapacityUnit> : IElevatorRepository<TCapacityUnit>
    {
        #region PRIVATE PROPERTIES

        private readonly IElevatorDBContext<TCapacityUnit> _context;
        private ILogger _logger;

        #endregion PRIVATE PROPERTIES


        #region CONSTRUCTORS

        public ElevatorRepository(IElevatorDBContext<TCapacityUnit> context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        #endregion CONSTRUCTORS


        #region PUBLIC METHODS

        /// <summary>
        /// Check that required fields are set and data is in correct format
        /// </summary>
        /// <param name="elevator"></param>
        /// <returns></returns>
        public Response ValidateElevator(Elevator<TCapacityUnit> elevator)
        {
            return new Response(true);
        }

        /// <summary>
        /// Create a new elevator in the database
        /// </summary>
        /// <param name="elevator"></param>
        /// <returns>Returns the created object's db ID (primary key), if created successfully</returns>
        public Response<int?> CreateElevator(Elevator<TCapacityUnit> elevator)
        {
            if (!ValidateElevator(elevator).Success)
            {
                string message = "oops";
                _logger.LogWarning(message);
                return new Response<int?>() { Success = false, Message = message, Data = null };
            }

            return _context.CreateElevator(elevator);
        }

        /// <summary>
        /// Deletes the specified elevator in the database, if found
        /// </summary>
        /// <param name="elevatorId"></param>
        /// <returns></returns>
        public Response DeleteElevator(int elevatorId)
        {
            return _context.DeleteElevator(elevatorId);
        }

        /// <summary>
        /// Get a list of all elevators in the database
        /// </summary>
        /// <returns></returns>
        public Response<List<Elevator<TCapacityUnit>>> GetAllElevators()
        {
            return _context.GetAllElevators();
        }

        /// <summary>
        /// Get a specific elevator from the database
        /// </summary>
        /// <param name="elevatorId"></param>
        /// <returns></returns>
        public Response<Elevator<TCapacityUnit>> GetElevator(int elevatorId)
        {
            return _context.GetElevator(elevatorId);
        }

        public Response<Elevator<TCapacityUnit>> UpdateElevator(Elevator<TCapacityUnit> elevator)
        {
            if (!ValidateElevator(elevator).Success)
            {
                string message = "oops";
                _logger.LogWarning(message);
                return new Response<Elevator<TCapacityUnit>>() {  Success = false, Message = message };
            }

            return _context.UpdateElevator(elevator);
        }

        #endregion PUBLIC METHODS
    }
}
