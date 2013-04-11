using System.Data;
using Curtin.Framework.Database.Migrations;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20111004132100)]
    public class RefactorDataSharingAvailabilityDescriptionFromStringToDate : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.ExecuteNonQuery("ALTER TABLE [DataManagementPlan] DROP COLUMN [DataSharingAvailabilityDescription]");
            Database.AddColumn("[DataManagementPlan]", new Column("DataSharingAvailabilityDate", DbType.DateTime));
        }

        public override void Down()
        {
            Database.ExecuteNonQuery("ALTER TABLE [DataManagementPlan] DROP COLUMN [DataSharingAvailabilityDate]");
            Database.AddColumn("[DataManagementPlan]", new Column("DataSharingAvailabilityDescription", DbType.String, DbString.MaxLength));
        }
    }
}
