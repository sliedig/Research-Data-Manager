using System.Data;
using Curtin.Framework.Database.Migrations;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20110826124700)]
    public class AddShareAccessDescription : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.AddColumn("[DataManagementPlan]", "DataSharingShareAccessDescription", DbType.String, DbString.MaxLength);
            

        }

        public override void Down()
        {
            Database.ExecuteNonQuery("ALTER TABLE [DataManagementPlan] DROP COLUMN DataSharingShareAccessDescription");
        }
    }
}
