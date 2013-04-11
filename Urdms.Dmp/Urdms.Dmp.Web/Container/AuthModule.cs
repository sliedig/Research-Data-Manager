using Autofac;
using Curtin.Framework.Common.Attributes;
using Urdms.Dmp.Web.Auth;

namespace Urdms.Dmp.Web.Container
{
    [NoCoverage]
    public class AuthModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AuthenticationService>().As<IAuthenticationService>().InstancePerLifetimeScope();
        }
    }
}
