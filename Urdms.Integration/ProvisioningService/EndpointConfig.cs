using NServiceBus;
using NServiceBus.Features;

namespace Urdms.ProvisioningService
{
    public class EndpointConfig : IConfigureThisEndpoint, AsA_Publisher, IWantCustomInitialization
    {
        public void Init()
        {
            Configure.With()
                     .DefaultBuilder();

            Configure.Features.Enable<Sagas>()
                .AutoSubscribe(settings => settings.DoNotAutoSubscribeSagas());
        }
    }

}
