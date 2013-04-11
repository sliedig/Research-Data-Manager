using System.ComponentModel;

namespace Urdms.DocumentBuilderService.Models.Enums
{
    public enum ShareAccess
    {
        None = 0,

        [Description("Open access")]
        Open = 1,

        [Description("Restricted access (might require username/password)")]
        Restricted = 2,

        Other = 3
    }
}