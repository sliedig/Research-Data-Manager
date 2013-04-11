using Autofac;
using Curtin.Framework.Common.Extensions;

namespace Urdms.Dmp.Config.Autofac
{
    public class DataAccessModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            var dataAccessAssembly = typeof(DataAccessModule).Assembly;

            builder.RegisterAssemblyTypes(dataAccessAssembly)
                .Where(t => t.Namespace.NullSafe().EndsWith("Database.Repositories"))
                .AsDefaultInterface().InstancePerLifetimeScope();
        }
    }
}
