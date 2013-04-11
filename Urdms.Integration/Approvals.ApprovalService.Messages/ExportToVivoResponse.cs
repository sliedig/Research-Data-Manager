using System;
using NServiceBus;

namespace Urdms.Approvals.ApprovalService.Messages
{
    public class ExportToVivoResponse : IMessage
    {
        public int DataCollectionId { get; set; }
        public DateTime RecordPublishedOn { get; set; }
    }
}