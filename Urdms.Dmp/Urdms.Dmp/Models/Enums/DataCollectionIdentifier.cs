using System.ComponentModel;

namespace Urdms.Dmp.Models.Enums
{
    public enum DataCollectionIdentifier
    {
        None = 0,
        [Description("Digital Object Identifier")]
        DigitalObjectIdentifier = 1,
        [Description("Handle Identifier")]
        HandleIdentifier = 2
    }
}