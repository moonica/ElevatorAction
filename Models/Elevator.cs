using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorAction.Models
{
    public abstract class Elevator<TCapacityUnit> : IElevator<TCapacityUnit> where TCapacityUnit : INumber<TCapacityUnit>
    {
        public static Dictionary<ElevatorTypeEnum, ElevatorCapacityUnits> ElevatorCapacityUnitMapper = new Dictionary<ElevatorTypeEnum, ElevatorCapacityUnits>()
        {
            { ElevatorTypeEnum.None, ElevatorCapacityUnits.undefined },
            { ElevatorTypeEnum.Passenger, ElevatorCapacityUnits.persons }
        };

        public int ElevatorID { get; set; }
        public string ElevatorName { get; set; }
        public int? CurrentFloor { get; set; }
        public ElevatorStatusEnum ElevatorMovingStatus { get; set; }
        public string Model { get; set; }
        public string SerialNr { get; set; }
        public TCapacityUnit CurrentCapacity { get; set; }
        public TCapacityUnit MaxCapacity { get; set; }

        public abstract bool HasRoomFor(TCapacityUnit ToAdd);

        public abstract bool IsFull();
    }
}
