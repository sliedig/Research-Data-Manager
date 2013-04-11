using System.ComponentModel;

namespace Urdms.DocumentBuilderService.Models.Enums
{
    // If changes are made here, the Integration project also needs updating.

    public enum DataRetentionPeriod
    {
        [Description("5 years  (standard retention period)")]
        Short = 0,

        [Description("15 years (clinical trials)")]
        Medium = 1,

        [Description("75 years (data that has potential long term effects such as the intervention with human objects or the environment)")]
        Long = 2,

        [Description("Permanently (data of high public interest or significance)")]
        Permanently = 3
    }
}