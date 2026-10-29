using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.Domain.Enums
{
    public enum CancellationReasons
    {
        PassengerChangedMind,
        DriverChangedMind,
        WrongPickupLocation,
        WrongDestination,
        PassengerNoShow,
        DangerousPassenger,
        VehicleIssue,
        Emergency,
        CancelledByAdmin,
        Other
    }
}
