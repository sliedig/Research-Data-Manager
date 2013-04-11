using System.ComponentModel;

namespace Urdms.Dmp.Models.Enums
{
    // If changes are made here, the Integration project also needs updating.

    public enum DataLicensingType
    {
        [Description("All Rights Reserved (copyright protection on research data)")]
        AllRightsReserved = 0,

        [Description("Creative Commons")]
        CreativeCommons = 1,

        [Description("Restrictive License")]
        RestrictiveLicence = 2,

        [Description("Contact Principal Investigator to determine licensing arrangement")]
        SeekFurtherInformation = 3
    }
}