using System;

namespace Curtin.Urdms.Approvals.ApprovalService.Messages
{
    public class SubmitForFinalApproval : IApprovalCommand
    {
        public int DataCollectionId { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime ApprovedOn { get; set; }
    }
}