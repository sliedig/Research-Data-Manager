using System.ComponentModel;

namespace Urdms.Dmp.Models.Enums
{
    public enum MaxDataSize
    {
        None = 0,
        [Description("Less than 100MB (small)")]
        Small = 1,
        [Description("Between 100MB to 2GB (medium)")]
        Medium = 2,
        [Description("More than 2GB (large)")]
        Large = 3
    }
}