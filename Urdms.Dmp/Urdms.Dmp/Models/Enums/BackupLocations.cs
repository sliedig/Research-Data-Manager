using System;
using System.ComponentModel;

namespace Urdms.Dmp.Models.Enums
{
    [Flags]
    public enum BackupLocations
    {
        [Description("Institutional Network Drives")]
        InstitutionalBackup = 0,
        [Description("Personal Hard Drives (External Hard Drives, USB, DVD, etc.)")]
        PersonalHardDrives = 1,
        Other = 2
    }
}