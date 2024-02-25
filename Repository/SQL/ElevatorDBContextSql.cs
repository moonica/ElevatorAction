using ElevatorAction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorAction.Repository
{
    public class ElevatorDBContext<TCapacityUnit> : IElevatorDBContext<TCapacityUnit>
    {
        #region INITIALIZE DB
        #endregion INITIALIZE DB


        #region OPERATIONS
        public Response<int?> CreateElevator(Elevator<TCapacityUnit> elevator)
        {
            throw new NotImplementedException();
        }

        public Response DeleteElevator(int elevatorId)
        {
            throw new NotImplementedException();
        }

        public Response<List<Elevator<TCapacityUnit>>> GetAllElevators()
        {
            throw new NotImplementedException();
        }

        public Response<Elevator<TCapacityUnit>> GetElevator(int elevatorId)
        {
            throw new NotImplementedException();
        }

        public Response<Elevator<TCapacityUnit>> UpdateElevator(Elevator<TCapacityUnit> elevator)
        {
            throw new NotImplementedException();
        }
        #endregion OPERATIONS
    }
}
