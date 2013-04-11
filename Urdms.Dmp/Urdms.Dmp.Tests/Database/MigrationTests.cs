using NUnit.Framework;
using Urdms.Dmp.Tests.Helpers;

namespace Urdms.Dmp.Tests.Database
{
    class MigrationsShould : DbTestBase
    {
        [Test]
        public void Go_down_and_up()
        {
            Migrator.MigrateTo(0);
            Migrator.MigrateToLatestVersion();
        }
    }
}
