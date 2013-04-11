using System.Data;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20110909095300)]
    public class RemoveAllocationColumns : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.ExecuteNonQuery("ALTER TABLE [ProjectSocioEconomicObjective] DROP COLUMN [Allocation]");
            Database.ExecuteNonQuery("ALTER TABLE [ProjectFieldOfResearch] DROP COLUMN [Allocation]");
        }

        public override void Down()
        {
            Database.AddColumn("[ProjectSocioEconomicObjective]", "Allocation", DbType.Double, ColumnProperty.NotNull);
            Database.AddColumn("[ProjectFieldOfResearch]", "Allocation", DbType.Double, ColumnProperty.NotNull);
        }
    }
}
