using System;
using System.ComponentModel;

namespace Urdms.Dmp.Models.Enums
{
    // If changes are made here, the Integration project also needs updating.

    [Flags]
    public enum DataRetentionLocations
    {
        None = 0,
        [Description("Institutional Repository")]
        Internal = 1,
        [Description("Unspecified repository external to Institution")]
        External = 2,
        [Description("Specific repository")]
        Other = 4
    }
}