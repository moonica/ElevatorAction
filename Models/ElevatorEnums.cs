using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorAction.Models
{
    /// <summary>
    /// Enum to describe the type of elevator (eg. freight, passenger). "None" is default or unknown type.
    /// </summary>
    public enum ElevatorTypeEnum
	{
		None = 0, 
		Passenger = 1
	}

    /// <summary>
    /// Enum to describe current movement status of an elevator (standing still, going up, going down etc). "Undefined" means an elevator has been created in the system but not yet used/moved since last reboot
    /// </summary>
    public enum ElevatorStatusEnum
	{
		Undefined, Stopped, GoingUp, GoingDown
	}	

    public enum ElevatorCapacityUnits
    {
        undefined,
        persons
    }
}
