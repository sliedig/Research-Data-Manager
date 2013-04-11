using System;
using System.ComponentModel;

namespace Urdms.DocumentBuilderService.Models.Enums
{
    [Flags]
    public enum BackupLocations
    {
        [Description("Institutional Microsoft SharePoint project site backup")]
        MicrosoftSharePoint = 0,
        [Description("Institutional backup")]
        InstitutionalBackup = 1,
        [Description("Personal hard drives")]
        PersonalHardDrives = 2,
        Other = 4
    }
}