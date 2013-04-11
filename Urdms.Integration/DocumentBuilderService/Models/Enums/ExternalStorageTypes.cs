using System;
using System.ComponentModel;

namespace Urdms.DocumentBuilderService.Models.Enums
{
    [Flags]
    public enum ExternalStorageTypes
    {
        None = 0,
        [Description("Data Fabric")]
        DataFabric = 1,
        [Description("iVec Petabyte")]
        IvecPetabyte = 2,
        [Description("Others")]
        Other = 4
    }
}