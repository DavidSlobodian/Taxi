using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.Domain.Enums
{
    public enum RatingTags
    {
        //passenger
        PunctualPassenger,
        PolitePassenger,
        CarefulPassenger,
        SafePassenger,
        GoodCommunicatorPassenger,

        LateComingPassenger,
        UnbehavedPassenger,
        DangerousPassenger,
        TooManyPeoplePassenger,
        PetWithoutWarningPassenger,

        //Driver
        PoliteDriver,
        CleanVehicle,
        CarefulDriving,
        ComfortAtmosphere,
        GoodCommunicatorDriver,
        GoodCityKnowingDriver,


        DirtyVehicle,
        UnpoliteDriver,
        DangerousDriving,
        SmokingDriver,
        ViolatingRulesDriver,
        IgnoringDriver,
        LateArrivalDriver,
        UnapproperiteBehaviorDriver
    }
}
