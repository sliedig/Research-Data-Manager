using System;
using NServiceBus;

namespace Curtin.Urdms.Approvals.ApprovalService.Messages
{
    public interface IApprovalCommand : ICommand
    {
        int DataCollectionId { get; set; }
        string ApprovedBy { get; set; }
        DateTime ApprovedOn { get; set; }
    }
}