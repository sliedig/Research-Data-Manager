using System.ComponentModel;

namespace Urdms.Dmp.Models.Enums
{
    // If changes are made here, the Integration project also needs updating.

    public enum ShareAccess
    {
        [Description("No access")]
        NoAccess = 0,

        [Description("Restricted access (might require username/password)")]
        Restricted = 2,

        [Description("Open access")]
        Open = 1,
        
        Other = 3
    }
}