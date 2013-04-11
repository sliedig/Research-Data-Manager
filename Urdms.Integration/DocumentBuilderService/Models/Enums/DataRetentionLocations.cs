using System;
using System.ComponentModel;

namespace Urdms.DocumentBuilderService.Models.Enums
{
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