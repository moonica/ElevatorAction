using ElevatorAction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorAction.Repository
{
    public interface IElevatorDBContext<TCapacityUnit> 
    {
        public Response<int?> CreateElevator(Elevator<TCapacityUnit> elevator);

        public Response DeleteElevator(int elevatorId);

        public Response<List<Elevator<TCapacityUnit>>> GetAllElevators();

        public Response<Elevator<TCapacityUnit>> GetElevator(int elevatorId);

        public Response<Elevator<TCapacityUnit>> UpdateElevator(Elevator<TCapacityUnit> elevator);
    }
}
