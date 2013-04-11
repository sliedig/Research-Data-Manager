using System.Data;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20111101133700)]
    public class ChangeRelationshipBetweenProjectAndDataManagementPlanTables : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.ExecuteNonQuery("ALTER TABLE [DataManagementPlan] DROP COLUMN [Status]");
            Database.AddColumn("Project","ProvisioningStatus", DbType.Int32);
            
        }

        public override void Down()
        {
            Database.ExecuteNonQuery("ALTER TABLE [Project] DROP COLUMN [ProvisioningStatus]");
            Database.AddColumn("DataManagementPlan", "Status", DbType.Int32);
        }
    }
}
