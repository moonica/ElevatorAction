using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorAction.UserInterface
{
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
    }
}
