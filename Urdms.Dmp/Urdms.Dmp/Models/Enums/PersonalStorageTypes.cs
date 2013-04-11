using System;
using System.ComponentModel;

namespace Urdms.Dmp.Models.Enums
{
    [Flags]
    public enum PersonalStorageTypes
    {
        None = 0,
        [Description("Internal Hard Drives (found in PCs, laptops, tablets and various mobile devices)")]
        InternalHardDrives = 1,
        [Description("Removable Media (includes CDs, DVD, USB sticks, and external hard drives)")]
        RemovableMedia = 2,
        [Description("Others")]
        Other = 4

    }
}