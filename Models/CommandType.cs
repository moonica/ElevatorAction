using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorAction.Models
{
    //NB: whenever this list gets changed, the object "UICommand" must get updated with the command's visibility and description
    public enum CommandType
    {
        Abort,
        CallElevator,
        CreateElevator,
        DeleteElevator,
        DisableElevator,
        EnableElevator,
        Exit,
        Help,
        ListElevatorDetails,
        ListElevators,
        ListElevatorStates,
        TryAgain,
        Test
    }
}


