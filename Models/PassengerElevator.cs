using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorAction.Models
{
    public class PassengerElevator : Elevator<int>, IElevator<int>
    {
        #region PRIVATE/PROTECTED PROPERTIES

        private ElevatorTypeEnum _elevatorType;
        private ElevatorCapacityUnits _capacityUnits;

        #endregion PRIVATE/PROTECTED PROPERTIES


        #region PUBLIC PROPERTIES

        /// <summary>
        /// Elevator type enum. Read only; gets set by constructor of derived classes. Null indicates base elevator type.
        /// </summary>
        public ElevatorTypeEnum ElevatorType
        {
            get
            {
                return _elevatorType;
            }
        }

        /// <summary>
        /// Readonly. Field that indicates what units the elevator's capacity is measured in
        /// </summary>
        public ElevatorCapacityUnits CapacityUnits
        {
            get 
            {
                return _capacityUnits;
            }
        }

        #endregion PUBLIC PROPERTIES


        #region CONSTRUCTORS

        public PassengerElevator() : base()
        {
            _elevatorType = ElevatorTypeEnum.Passenger;
            _capacityUnits = ElevatorCapacityUnitMapper[ElevatorTypeEnum.Passenger];
        }

        #endregion CONSTRUCTORS


        #region PUBLIC METHODS

        public override bool IsFull()
        {
            return CurrentCapacity >= MaxCapacity;
        }

        /// <summary>
        /// Determine if the elevator has enough capacity for the given additional passengers
        /// </summary>
        /// <param name="ToAdd">The number of passengers to be added without exceeding capacity</param>
        /// <returns>Returns a boolean indicating if the elevator has the requested capacity available</returns>
        public override bool HasRoomFor(int ToAdd)
        {
            return (CurrentCapacity + ToAdd) <= MaxCapacity;
        }

        #endregion PUBLIC METHODS

    }
}
