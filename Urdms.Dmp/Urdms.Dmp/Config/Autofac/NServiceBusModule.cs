using Autofac;
using NServiceBus;

namespace Urdms.Dmp.Config.Autofac
{
    public class NServiceBusModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(c =>
                NServiceBus.Configure
                .WithWeb()
                .DefineEndpointName("urdms.web")
                .Log4Net()
                .DefaultBuilder()
                .XmlSerializer()
                .MsmqTransport()
                    .IsTransactional(false)
                    .PurgeOnStartup(false)
                .UnicastBus()
                    .ImpersonateSender(false)
                .CreateBus()
                .Start(() => NServiceBus.Configure.Instance.ForInstallationOn<NServiceBus.Installation.Environments.Windows>().Install()))
               .As<IBus>()
               .SingleInstance();

        }
    }
}