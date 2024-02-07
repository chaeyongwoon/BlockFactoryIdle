using System.Collections.Generic;

namespace MondayOFF
{
    public static partial class RemoteConfig
    {
        public static readonly Dictionary<string, object> DefaultValues = new()
        {
            {"IsInterval", 120},

            //{"MutationRateHigh", false},
            //{"MutationRateFever", false},
            //{"MutationRateFeverType3", 0},
            //{"HuntUnitGroupCountLimit", true},
        };
    }
}