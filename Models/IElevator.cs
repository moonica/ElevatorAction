using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorAction.Models
{
    public interface IElevator<TCapacityUnit> where TCapacityUnit : INumber<TCapacityUnit>
    {
        #region PUBLIC PROPERTIES

        /// <summary>
        /// Elevator id / primary key
        /// </summary>
        public int ElevatorID { get; set; }

        /// <summary>
        /// Elevator human readable name
        /// </summary>
        public string ElevatorName { get; set; }

        /// <summary>
        /// Last known position of the elevator (as a floor number). Use field <ref>ElevatorMovingStatus</ref> to validate if the elevator is AT that floor, or moving away from it. "null" means an elevator has been created in the system but not yet used since last reboot
        /// </summary>
        public int? CurrentFloor { get; set; }

        /// <summary>
        /// Elevator moving status enum (standing still, going up, going down etc. "Undefined" means an elevator has been created in the system but not yet used since last reboot
        /// </summary>
        public ElevatorStatusEnum ElevatorMovingStatus { get; set; }

        /// <summary>
        /// Elevator model number from supplier
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// Elevator unique serial number from supplier
        /// </summary>
        public string SerialNr { get; set; }

        /// <summary>
        /// Current carrying capacity of the elevator
        /// </summary>
        public TCapacityUnit CurrentCapacity { get; set; }

        /// <summary>
        /// Maximum capacity of the elevator
        /// </summary>
        public TCapacityUnit MaxCapacity { get; set; }

        #endregion PUBLIC PROPERTIES



        #region PUBLIC METHODS

        /// <summary>
        /// Determine if the elevator has any capacity open
        /// </summary>
        /// <returns>Returns a boolean indicating if the elevator is full or not</returns>
        public bool IsFull();

        public bool HasRoomFor(TCapacityUnit ToAdd);
        #endregion PUBLIC METHODS
    }
}
