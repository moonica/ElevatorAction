using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorAction.Models
{
    public abstract class Elevator<TCapacityUnit>
    {
        #region PUBLIC PROPERTIES

        public static Dictionary<ElevatorTypeEnum, ElevatorCapacityUnits> ElevatorCapacityUnitMapper = new Dictionary<ElevatorTypeEnum, ElevatorCapacityUnits>()
        {
            { ElevatorTypeEnum.None, ElevatorCapacityUnits.undefined },
            { ElevatorTypeEnum.Passenger, ElevatorCapacityUnits.persons }
        };

        /// <summary>
        /// Elevator id / primary key
        /// </summary>
        public int ElevatorID;

        /// <summary>
        /// Elevator human readable name
        /// </summary>
        public string ElevatorName;

        /// <summary>
        /// Last known position of the elevator (as a floor number). Use field <ref>ElevatorMovingStatus</ref> to validate if the elevator is AT that floor, or moving away from it. "null" means an elevator has been created in the system but not yet used since last reboot
        /// </summary>
        public int? CurrentFloor;

        /// <summary>
        /// Elevator moving status enum (standing still, going up, going down etc. "Undefined" means an elevator has been created in the system but not yet used since last reboot
        /// </summary>
        public ElevatorStatusEnum ElevatorMovingStatus;

        /// <summary>
        /// Elevator model number from supplier
        /// </summary>
        public string Model;

        /// <summary>
        /// Elevator unique serial number from supplier
        /// </summary>
        public string SerialNr;

        /// <summary>
        /// Current carrying capacity of the elevator
        /// </summary>
        public TCapacityUnit CurrentCapacity;

        /// <summary>
        /// Maximum capacity of the elevator
        /// </summary>
        public TCapacityUnit MaxCapacity;


        #endregion PUBLIC PROPERTIES


        #region CONSTRUCTORS

        /// <summary>
        /// Create a new instance of an elevator. This base type does not have a set type. It is advisable to use a derived class with a specific type.
        /// </summary>
        public Elevator()
        { }

        #endregion CONSTRUCTORS


        #region PUBLIC METHODS

        /// <summary>
        /// Determine if the elevator has any capacity open
        /// </summary>
        /// <returns>Returns a boolean indicating if the elevator is full or not</returns>
        public abstract bool IsFull();

        public abstract bool HasRoomFor(TCapacityUnit ToAdd);
        #endregion PUBLIC METHODS
    }
}
