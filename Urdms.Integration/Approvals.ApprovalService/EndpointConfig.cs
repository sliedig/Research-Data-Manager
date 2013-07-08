using NServiceBus;
using NServiceBus.Features;

namespace Urdms.Approvals.ApprovalService
{
    public class EndpointConfig : IConfigureThisEndpoint, AsA_Publisher, IWantCustomInitialization
    {
        public void Init()
        {
            Configure.With()
               .DefaultBuilder()
               .UseInMemoryTimeoutPersister()
               .UseInMemoryGatewayPersister()
               .DBSubcriptionStorage()
                .NHibernateSagaPersister()
                   .NHibernateUnitOfWork()
                .UnicastBus()
                   .LoadMessageHandlers();

            Configure.Serialization.Xml();
            Configure.Features.Enable<Sagas>().AutoSubscribe(settings => settings.DoNotAutoSubscribeSagas());
        }
    }
}