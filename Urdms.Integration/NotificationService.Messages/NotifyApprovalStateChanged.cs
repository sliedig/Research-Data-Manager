using NServiceBus;

namespace Urdms.NotificationService.Messages
{
    public class NotifyApprovalStateChanged : IMessage
    {
        public int DataCollectionId { get; set; }
        public string ApprovalState { get; set; }
        public string Approver { get; set; }
    }
}