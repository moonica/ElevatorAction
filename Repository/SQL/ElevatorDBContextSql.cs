using ElevatorAction.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using ElevatorAction.Repository.SQL;
using System.Diagnostics.Tracing;

namespace ElevatorAction.Repository
{
    public class ElevatorDBContext<TCapacityUnit> : IElevatorDBContext<TCapacityUnit>
    {
        #region INITIALIZE DB

        private ILogger _logger;
        private SqlDbContext _context;

        //List of stored procedure names for ease of reference and maintenance
        private readonly string spCreate = "spCreateElevator";
        private readonly string spDelete = "spDeleteElevator";
        private readonly string spGetElevators = "spGetElevators";
        private readonly string spUpdateElevator = "spUpdateElevator";

        public ElevatorDBContext(ILogger logger, string connectionString, int? commandTimeout = null)
        {
            _logger = logger;
            _context = new SqlDbContext(connectionString, logger, commandTimeout);
        }

        #endregion INITIALIZE DB


        #region PRIVATE METHODS        

        private Dictionary<string, object> createParameterList(Elevator<TCapacityUnit> elevator)
        {
            return new Dictionary<string, object>();
        }

        #endregion PRIVATE METHODS


        #region OPERATIONS
        public Response<int?> CreateElevator(Elevator<TCapacityUnit> elevator)
        {
            if (elevator == null)
                return new Response<int?>(false, "Elevator object to create not provided");
            
            using (_context)
            {
                var createResponse = _context.GetSqlScalarValueFromSP(spCreate, createParameterList(elevator));

                if (!createResponse.Success)
                    return new Response<int?>() { Success = false, Message = $"Elevator not created; an unexpected error was encountered ('{createResponse.Message}')" };

                if (!int.TryParse(createResponse.Data?.ToString(), out int intResponse) || (intResponse < 1))
                {
                    _logger.LogError($"DB insert elevator returned success but invalid new ID returned ('{createResponse.Data}')");
                    return new Response<int?>() { Success = false, Message = "Elevator may not have been created; could not retrieve new elevator ID from database" };
                }

                return new Response<int?>(intResponse);
            }
        }

        public Response DeleteElevator(int elevatorId)
        {
            using (_context)
            {
                var deleteResponse = _context.GetSqlScalarValueFromSP(spDelete, new Dictionary<string, object>() { { "ElevatorID", elevatorId } });

                if (!deleteResponse.Success)
                    return new Response<int?>() { Success = false, Message = $"Elevator with ID {elevatorId} not deleted; an unexpected error was encountered ('{deleteResponse.Message}')" };

                if (!int.TryParse(deleteResponse.Data?.ToString(), out int intResponse) || (intResponse < 1))
                {
                    _logger.LogError($"DB delete for elevator ID {elevatorId} returned success but invalid nr rows affected returned ('{deleteResponse.Data}')");
                    return new Response<int?>() { Success = false, Message = "Elevator not deleted; an unexpected error was encountered" };
                }

                return new Response<int?>(intResponse);
            }
        }

        public Response UpdateElevator(Elevator<TCapacityUnit> elevator)
        {
            if (elevator == null)
                return new Response(false, "Elevator object to update not provided");

            using (_context)
            {
                var deleteResponse = _context.GetSqlScalarValueFromSP(spUpdateElevator, createParameterList(elevator));

                if (!deleteResponse.Success)
                    return new Response<int?>() { Success = false, Message = $"Elevator with ID {elevator.ElevatorID} not updated; an unexpected error was encountered ('{deleteResponse.Message}')" };

                if (!int.TryParse(deleteResponse.Data?.ToString(), out int intResponse) || (intResponse < 1))
                {
                    _logger.LogError($"DB update for elevator ID {elevator.ElevatorID} returned success but invalid nr rows affected returned ('{deleteResponse.Data}')");
                    return new Response<int?>() { Success = false, Message = $"Elevator with ID {elevator.ElevatorID} not updated; an unexpected error was encountered" };
                }

                return new Response<int?>(intResponse);
            }
        }

        public Response<List<TElevatorImplementation>> GetAllElevators<TElevatorImplementation>() where TElevatorImplementation : Elevator<TCapacityUnit>, new()
        {
            using (_context)
            {
                var getAllResponse = _context.GetSqlResultListFromSP<TElevatorImplementation>(spGetElevators);

                if (!getAllResponse.Success)
                    return new Response<List<TElevatorImplementation>>() { Success = false, Message = $"Could not retrieve elevators; an unexpected error was encountered ('{getAllResponse.Message}')" };

                return getAllResponse;
            }
        }

        public Response<TElevatorImplementation> GetElevator<TElevatorImplementation>(int elevatorId) where TElevatorImplementation : Elevator<TCapacityUnit>, new()
        {
            using (_context)
            {
                var getElevatorResponse = _context.GetSqlResultFromSP<TElevatorImplementation>(spGetElevators, new Dictionary<string, object>() { { "ElevatorID", elevatorId } });

                if (!getElevatorResponse.Success)
                    return new Response<TElevatorImplementation>() { Success = false, Message = $"Could not retrieve elevator with id {elevatorId}; an unexpected error was encountered ('{getElevatorResponse.Message}')" };

                return getElevatorResponse;
            }
        }

        #endregion OPERATIONS
    }
}
