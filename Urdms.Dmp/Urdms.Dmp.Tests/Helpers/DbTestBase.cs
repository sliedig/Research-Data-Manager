using System;
using System.Configuration;
using System.Reflection;
using Curtin.Framework.Test.Database;
using Urdms.Dmp.Config.NHibernate;

namespace Urdms.Dmp.Tests.Helpers
{
    public class DbTestBase : DbTestBase<NHibernateConfiguration>
    {
        protected override Tuple<string, Assembly[]> GetConfig()
        {
            return new Tuple<string, Assembly[]>(ConfigurationManager.AppSettings["Database"], new[] { typeof(NHibernateConfiguration).Assembly });
        }
    }
}
