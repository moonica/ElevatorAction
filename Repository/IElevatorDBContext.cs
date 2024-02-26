using ElevatorAction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorAction.Repository
{
    public interface IElevatorDBContext<TCapacityUnit> where TCapacityUnit : INumber<TCapacityUnit>
    {
        public Response<int?> CreateElevator(IElevator<TCapacityUnit> elevator);

        public Response DeleteElevator(int elevatorId);

        public Response<List<IElevator<TCapacityUnit>>> GetAllElevators<TImplementation>() where TImplementation : IElevator<TCapacityUnit>, new();

        public Response<IElevator<TCapacityUnit>> GetElevator<TImplementation>(int elevatorId) where TImplementation : IElevator<TCapacityUnit>, new();

        public Response UpdateElevator(IElevator<TCapacityUnit> elevator);
    }
}
