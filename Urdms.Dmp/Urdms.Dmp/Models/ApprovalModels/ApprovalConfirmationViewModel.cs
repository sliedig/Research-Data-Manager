using System.ComponentModel.DataAnnotations;
using Urdms.Dmp.Models.Enums;

namespace Urdms.Dmp.Models.ApprovalModels
{
    public class ApprovalConfirmationViewModel
    {
        public int DataCollectionId { get; set; }
        public string Title { get; set; }

        [Display(Name = "I hereby confirm that this data collection is acceptable. It can proceed to secondary approval.")]
        public bool IsQaApproved { get; set; }

        [Display(Name = "I hereby confirm that this data collection does not violate the project's contractual agreements.")]
        public bool DoesNotViolateAgreements { get; set; }

        [Display(Name = "I hereby confirm that this data collection does not violate confidentiality and ethics requirements.")]
        public bool DoesNotViolateConfidentialityAndEthics { get; set; }

        [Display(Name = "This data collection description has been reviewed by the QA and Secondary approver. There is obligation to making this data collection description publicly available via Research Data Australia.")]
        public bool IsPublicationApproved { get; set; }

        public DataCollectionStatus State { get; set; }
        public bool IsChanged { get; set; }
        public DataCollectionStatus ProposedState { get; set; }
    }
}