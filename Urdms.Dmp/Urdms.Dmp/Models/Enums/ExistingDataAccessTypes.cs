using System;
using System.ComponentModel;

namespace Urdms.Dmp.Models.Enums
{
    [Flags]
    public enum ExistingDataAccessTypes
    {
        None = 0,
        Purchase = 1,
        [Description("Obtain approval from owner")]
        ObtainApprovalFromOwner = 2,
        [Description("Data is freely available")]
        DataIsFreelyAvailable = 4,
        Other = 8
    }
}