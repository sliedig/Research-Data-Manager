using NServiceBus;

namespace Urdms.Approvals.VivoPublisher.Messages
{
    public class ExportToVivo : IMessage
    {
        public int DataCollectionId { get; set; }
    }
}