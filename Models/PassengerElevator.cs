using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorAction.Models
{
    internal class PassengerElevator : Elevator<int>
    {
        #region PUBLIC PROPERTIES

        /// <summary>
        /// Current carrying capacity of the elevator, measured as number of persons
        /// </summary>
        internal int CurrentCapacity_pax;

        /// <summary>
        /// Maximum capacity of the elevator, measured as number of persons
        /// </summary>
        internal int MaxCapacity_pax;

        #endregion PUBLIC PROPERTIES


        #region CONSTRUCTORS

        internal PassengerElevator() : base()
        {
            _elevatorType = ElevatorTypeEnum.Passenger;
        }

        #endregion CONSTRUCTORS


        #region PUBLIC METHODS

        internal override bool IsFull()
        {
            return CurrentCapacity_pax >= MaxCapacity_pax;
        }

        /// <summary>
        /// Determine if the elevator has enough capacity for the given additional passengers
        /// </summary>
        /// <param name="ToAdd">The number of passengers to be added without exceeding capacity</param>
        /// <returns>Returns a boolean indicating if the elevator has the requested capacity available</returns>
        internal override bool HasRoomFor(int ToAdd)
        {
            return (CurrentCapacity_pax + ToAdd) <= MaxCapacity_pax;
        }

        #endregion PUBLIC METHODS

    }
}
