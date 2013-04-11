using System.Data;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20110825133200)]
    public class AddHighestCompletedStepToDataManagementPlanTable : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.AddColumn("[DataManagementPlan]", "HighestCompletedStep", DbType.Int32, 1);
        }

        public override void Down()
        {
            Database.RemoveColumn("[DataManagementPlan]", "HighestCompletedStep");
        }
    }
}
