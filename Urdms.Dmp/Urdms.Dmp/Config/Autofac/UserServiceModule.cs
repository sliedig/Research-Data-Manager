using Autofac;
using Curtin.Framework.Common.UserService;
using Urdms.Dmp.Utils;

namespace Urdms.Dmp.Config.Autofac
{
	public class UserServiceModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);
			builder.RegisterType<DummyUserLookupService>().As<ICurtinUserService>().SingleInstance();	// TODO: Put your user service implementation here
		}
	}
}