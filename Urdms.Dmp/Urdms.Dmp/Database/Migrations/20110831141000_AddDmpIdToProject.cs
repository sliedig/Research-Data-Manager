using System.Data;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20110831141000)]
    public class AddDmpIdToProject : MigratorDotNetMigration
    {
        public override void Up()
        {
             Database.AddColumn("[Project]",
               new Column("DataManagementPlanId", DbType.Int32, ColumnProperty.NotNull));

             Database.ExecuteNonQuery("ALTER TABLE [DataManagementPlan] DROP COLUMN ProjectId");
        }

        public override void Down()
        {
            Database.AddColumn("[DataManagementPlan]",
               new Column("ProjectId", DbType.Int32, ColumnProperty.NotNull));

            Database.ExecuteNonQuery("ALTER TABLE [Project] DROP COLUMN DataManagementPlanId");
        }
    }
}
