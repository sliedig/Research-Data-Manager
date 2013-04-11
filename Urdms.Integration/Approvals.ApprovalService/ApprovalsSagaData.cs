using System;
using NServiceBus.Saga;
using Urdms.Approvals.ApprovalService.Events;

namespace Urdms.Approvals.ApprovalService
{
    public class ApprovalsSagaData : IContainSagaData
    {
        public virtual Guid Id { get; set; }
        public virtual string Originator { get; set; }
        public virtual string OriginalMessageId { get; set; }
        
        public virtual string CollectionOwner { get; set; }
        public virtual int DataCollectionId { get; set; }
        public virtual DateTime StateChangedOn { get; set; }
        public virtual string Approver { get; set; }
        public virtual DataCollectionApprovalState ApprovalState { get; set; }

    }
}