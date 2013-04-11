using System.ComponentModel;

namespace Urdms.Dmp.Models.Enums
{
    public enum DataCollectionStatus
    {
        Draft,

        [Description("Pending QA Approval")]
        Submitted,
        
        [Description("Pending Secondary Approval")]
        QaApproved,
        
        [Description("Pending Public Release")]
        SecondaryApproved,

		[Description("Pending Secondary Re-Approval")]
        RecordAmended,

		[Description("Pending Publishing")]
		Publishing,

		[Description("Published")]
		Published
    }
}