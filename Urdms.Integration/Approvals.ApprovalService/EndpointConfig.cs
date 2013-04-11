using NServiceBus;

namespace Urdms.Approvals.ApprovalService
{
    public class EndpointConfig : IConfigureThisEndpoint, AsA_Publisher, IWantCustomInitialization
    {
        public void Init()
        {
            Configure.With()
               .DefaultBuilder()
               .XmlSerializer()
               .RunTimeoutManagerWithInMemoryPersistence()
               .DBSubcriptionStorage()
                .Sagas()
                   .NHibernateSagaPersister()
                   .NHibernateUnitOfWork()
                .UnicastBus()
                   .DoNotAutoSubscribe()
                   .LoadMessageHandlers();
        }
    }
}