using System;
using System.ComponentModel;

namespace Urdms.DocumentBuilderService.Models.Enums
{
    [Flags]
    public enum VersionControl
    {
        None = 0,
        [Description("Filename will contain version control information")]
        File = 1,
        [Description("Software or system will manage version control")]
        System = 2,
        [Description("Other methods")]
        Other = 4
    }
}