using ElevatorAction.Models;
using ElevatorAction.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorTests.Mock_Objects
{
    internal class TestElevatorDBContext : IElevatorDBContext<int>
    {
        private List<TElevatorImplementation> generateElevators<TElevatorImplementation>(int count) where TElevatorImplementation : IElevator<int>, new()
        {
            var result = new List<TElevatorImplementation>();

            for (int i = 0; i < count; i++)
            {
                result.Add(new TElevatorImplementation()
                {
                    ElevatorID = i,
                    ElevatorName = $"Elevator{i}",
                    MaxCapacity = i + 10,
                    Model = DateTime.Now.TimeOfDay.ToString(),
                    SerialNr = $"{i}{i}{i}{i}{i}"
                });
            }

            return result;
        }


        public Response<int?> CreateElevator(IElevator<int> elevator)
        {
            return new Response<int?>() { Success = true, Data = 101 };
        }

        public Response DeleteElevator(int elevatorId)
        {
            return new Response(true);
        }

        public Response<List<IElevator<int>>> GetAllElevators<TImplementation>() where TImplementation : IElevator<int>, new()
        {
            var result = generateElevators<TImplementation>(5);

            return new Response<List<IElevator<int>>>()
            {
                Success = true,
                Data = result.Select(x => (IElevator<int>)x).ToList()
            };
        }

        public Response<IElevator<int>> GetElevator<TImplementation>(int elevatorId) where TImplementation : IElevator<int>, new()
        {
            var result = generateElevators<TImplementation>(1).FirstOrDefault();

            return new Response<IElevator<int>()
            {
                Success = true,
                Data = (IElevator<int>)result
            };
        }

        public Response UpdateElevator(IElevator<int> elevator)
        {
            return new Response(true);
        }
    }
}
