using System.ComponentModel;

namespace Urdms.Dmp.Models.Enums
{
    public enum DataRelationship
    {
        None,
        [Description("Capture new data in the same format as pre-existing data")]
        ExistingFormat,
        [Description("Transform pre-existing data to match format of new data")]
        NewFormat,
        [Description("Transform both new and pre-existing data to a unified format")]
        UnifiedFormat 
    }
}