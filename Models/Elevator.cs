using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorAction.Models
{
    internal abstract class Elevator<TCapacityUnit>
    {
        #region PRIVATE/PROTECTED PROPERTIES
        protected ElevatorTypeEnum _elevatorType;
        #endregion PRIVATE/PROTECTED PROPERTIES


        #region PUBLIC PROPERTIES

        /// <summary>
        /// Elevator id / primary key
        /// </summary>
        internal int ElevatorID;

        /// <summary>
        /// Elevator human readable name
        /// </summary>
        internal string ElevatorName;

        /// <summary>
        /// Elevator type enum. Read only; gets set by constructor of derived classes. Null indicates base elevator type.
        /// </summary>
        internal ElevatorTypeEnum ElevatorType
        {
            get
            {
                return _elevatorType;
            }
        }

        /// <summary>
        /// Last known position of the elevator (as a floor number). Use field <ref>ElevatorMovingStatus</ref> to validate if the elevator is AT that floor, or moving away from it. "null" means an elevator has been created in the system but not yet used since last reboot
        /// </summary>
        internal int? CurrentFloor;

        /// <summary>
        /// Elevator moving status enum (standing still, going up, going down etc. "Undefined" means an elevator has been created in the system but not yet used since last reboot
        /// </summary>
        internal ElevatorStatusEnum ElevatorMovingStatus;
        
        #endregion PUBLIC PROPERTIES


        #region CONSTRUCTORS
        /// <summary>
        /// Create a new instance of an elevator. This base type does not have a set type. It is advisable to use a derived class with a specific type.
        /// </summary>
        internal Elevator()
        {
            _elevatorType = Models.ElevatorTypeEnum.None;
        }

        #endregion CONSTRUCTORS


        #region PUBLIC METHODS

        /// <summary>
        /// Determine if the elevator has any capacity open
        /// </summary>
        /// <returns>Returns a boolean indicating if the elevator is full or not</returns>
        internal abstract bool IsFull();

        internal abstract bool HasRoomFor(TCapacityUnit ToAdd);
        #endregion PUBLIC METHODS
    }
}
