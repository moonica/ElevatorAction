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
using System.Numerics;

namespace ElevatorAction.Repository
{
    public class ElevatorDBContext<TCapacityUnit> : IElevatorDBContext<TCapacityUnit> where TCapacityUnit : INumber<TCapacityUnit>
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

        private Dictionary<string, object> createParameterList(IElevator<TCapacityUnit> elevator)
        {
            return new Dictionary<string, object>();
        }

        #endregion PRIVATE METHODS


        #region OPERATIONS
        public Response<int?> CreateElevator(IElevator<TCapacityUnit> elevator)
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

        public Response UpdateElevator(IElevator<TCapacityUnit> elevator)
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

        public Response<List<IElevator<TCapacityUnit>>> GetAllElevators<TElevator>() where TElevator : IElevator<TCapacityUnit>, new()
        {
            using (_context)
            {
                var getAllResponse = _context.GetSqlResultListFromSP<TElevator>(spGetElevators);

                if (!getAllResponse.Success)
                    return new Response<List<IElevator<TCapacityUnit>>>() { Success = false, Message = $"Could not retrieve elevators; an unexpected error was encountered ('{getAllResponse.Message}')" };

                var result = getAllResponse.Data.Select<TElevator, IElevator<TCapacityUnit>>(x => (IElevator<TCapacityUnit>)x);

                return new Response<List<IElevator<TCapacityUnit>>>()
                {
                    Success = getAllResponse.Success,
                    Message = getAllResponse.Message,
                    Data = result.ToList()
                };
            }
        }

        public Response<IElevator<TCapacityUnit>> GetElevator<TImplementation>(int elevatorId) where TImplementation : IElevator<TCapacityUnit>, new()
        {
            using (_context)
            {
                var getElevatorResponse = _context.GetSqlResultFromSP<TImplementation>(spGetElevators, new Dictionary<string, object>() { { "ElevatorID", elevatorId } });

                if (!getElevatorResponse.Success)
                    return new Response<IElevator<TCapacityUnit>>() { Success = false, Message = $"Could not retrieve elevator with id {elevatorId}; an unexpected error was encountered ('{getElevatorResponse.Message}')" };

                return new Response<IElevator<TCapacityUnit>>()
                {
                    Success = getElevatorResponse.Success,
                    Message = getElevatorResponse.Message,
                    Data = getElevatorResponse.Data
                };
            }
        }

        #endregion OPERATIONS
    }
}
