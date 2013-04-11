using Autofac;
using Autofac.Integration.Mvc;
using NHibernate;
using Urdms.Dmp.Config.NHibernate;

namespace Urdms.Dmp.Config.Autofac
{
    public class NHibernateModule : Module
    {
        public string ConnectionString { get; set; }
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            var nhConfiguration = new NHibernateConfiguration(ConnectionString);
            builder.Register(c => nhConfiguration.GetSessionFactory()).As<ISessionFactory>().SingleInstance();
            builder.Register(c => c.Resolve<ISessionFactory>().OpenSession()).As<ISession>().InstancePerHttpRequest();
        }
    }
}
