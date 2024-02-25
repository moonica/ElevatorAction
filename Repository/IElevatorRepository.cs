using ElevatorAction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorAction.Repository
{
    public interface IElevatorRepository<TCapacityUnit> 
    {
        public Response ValidateElevator(Elevator<TCapacityUnit> elevator);

        public Response<int?> CreateElevator(Elevator<TCapacityUnit> elevator);

        public Response<TElevatorImplementation> GetElevator<TElevatorImplementation>(int elevatorId) where TElevatorImplementation : Elevator<TCapacityUnit>, new();

        public Response<List<TElevatorImplementation>> GetAllElevators<TElevatorImplementation>() where TElevatorImplementation : Elevator<TCapacityUnit>, new();

        public Response UpdateElevator(Elevator<TCapacityUnit> elevator);

        public Response DeleteElevator(int elevatorId);
    }
}
