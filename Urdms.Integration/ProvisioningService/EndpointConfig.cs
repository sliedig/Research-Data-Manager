using NServiceBus;

namespace Urdms.ProvisioningService
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
