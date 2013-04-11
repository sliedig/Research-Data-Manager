using System.Data;
using Curtin.Framework.Database.Migrations;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20110825150700)]
    public class UpdateDataSharing : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.ExecuteNonQuery("ALTER TABLE [DataManagementPlan] DROP COLUMN ReuseByOrganisations");
            Database.AddColumn("[DataManagementPlan]", "DataSharingReuseByOrganisations", DbType.String, DbString.MaxLength);
            

        }

        public override void Down()
        {
            Database.ExecuteNonQuery("ALTER TABLE [DataManagementPlan] DROP COLUMN DataSharingReuseByOrganisations");
            Database.AddColumn("[DataManagementPlan]", "ReuseByOrganisations", DbType.String, DbString.MaxLength);
        }
    }
}
