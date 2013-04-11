using System;
using NServiceBus;

namespace Urdms.Approvals.ApprovalService.Commands
{
    public interface IApprovalCommand : ICommand
    {
        int DataCollectionId { get; set; }
        string ApprovedBy { get; set; }
        DateTime ApprovedOn { get; set; }
    }
}