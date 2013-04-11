using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20110818084500)]
    public class AlterDmpTableToDataManagementPlan : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.RenameTable("[Dmp]", "DataManagementPlan");
        }

        public override void Down()
        {
            Database.RenameTable("[DataManagementPlan]", "Dmp");
        }
    }
}
