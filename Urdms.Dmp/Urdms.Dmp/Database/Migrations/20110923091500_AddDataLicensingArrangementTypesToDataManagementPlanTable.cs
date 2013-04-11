using System.Data;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20110923091500)]
    public class AddDataLicensingArrangementTypesToDataManagementPlanTable : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.AddColumn("[DataManagementPlan]", "[DataSharingLicensingArrangement]", DbType.Int32);
        }

        public override void Down()
        {
            Database.RemoveColumn("[DataManagementPlan]", "[DataSharingLicensingArrangement]");
        }
    }
}
