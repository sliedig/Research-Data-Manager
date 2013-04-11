using System.Data;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20110818085500)]
    public class RenameDmpIdColumnToDataManagementPlanId : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.ExecuteNonQuery("ALTER TABLE [CurtinUser] DROP COLUMN [DmpId]");
            Database.ExecuteNonQuery("ALTER TABLE [NonCurtinUser] DROP COLUMN [DmpId]");

            Database.AddColumn("[CurtinUser]", "DataManagementPlanId", DbType.Int32);
            Database.AddColumn("[NonCurtinUser]", "DataManagementPlanId", DbType.Int32);


        }

        public override void Down()
        {
            Database.ExecuteNonQuery("ALTER TABLE [CurtinUser] DROP COLUMN [DataManagementPlanId]");
            Database.ExecuteNonQuery("ALTER TABLE [NonCurtinUser] DROP COLUMN [DataManagementPlanId]");

            Database.AddColumn("[CurtinUser]", "DmpId", DbType.Int32, ColumnProperty.NotNull);
            Database.AddColumn("[NonCurtinUser]", "DmpId", DbType.Int32, ColumnProperty.NotNull);
        }
    }
}
