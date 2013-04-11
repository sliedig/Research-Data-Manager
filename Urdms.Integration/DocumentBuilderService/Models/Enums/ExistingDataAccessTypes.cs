using System;
using System.ComponentModel;

namespace Urdms.DocumentBuilderService.Models.Enums
{
    [Flags]
    public enum ExistingDataAccessTypes
    {
        None = 0,
        Purchase = 1,
        [Description("Obtain Approval From Owner")]
        ObtainApprovalFromOwner = 2,
        [Description("Data Is Freely Available")]
        DataIsFreelyAvailable = 4,
        Other = 8
    }
}