using System.Data;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20110825120300)]
    public class DataRelationshipDetail : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.ExecuteNonQuery("ALTER TABLE [DataManagementPlan] DROP COLUMN RelationshipBetweenExistingAndNewData");
            Database.AddColumn("[DataManagementPlan]", "DataRelationshipDetailRelationshipBetweenExistingAndNewData", DbType.Int32, 0);
            

        }

        public override void Down()
        {
            Database.ExecuteNonQuery("ALTER TABLE [DataManagementPlan] DROP COLUMN DataRelationshipDetailRelationshipBetweenExistingAndNewData");
            Database.AddColumn("[DataManagementPlan]", "RelationshipBetweenExistingAndNewData", DbType.Int32, 0);
        }
    }
}
