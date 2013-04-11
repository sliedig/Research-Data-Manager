using System;
using NServiceBus;

namespace Urdms.Approvals.ApprovalService.Events
{
    public interface ApprovalStateChanged : IEvent
    {
        int DataCollectionId { get; set; }
        DataCollectionApprovalState ApprovalState { get; set; }
        DateTime StateChangedOn { get; set; }
        string Approver { get; set; }
    }
}