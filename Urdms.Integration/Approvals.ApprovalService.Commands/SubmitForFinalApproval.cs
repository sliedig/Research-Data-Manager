using System;

namespace Urdms.Approvals.ApprovalService.Commands
{
    public class SubmitForFinalApproval : IApprovalCommand
    {
        public int DataCollectionId { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime ApprovedOn { get; set; }
    }
}