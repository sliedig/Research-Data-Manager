using System.Data;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20110901143700)]
    public class MakeDmpIdNullable : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.ExecuteNonQuery("ALTER TABLE [Project] DROP COLUMN DataManagementPlanId");

            Database.AddColumn("[Project]", new Column("DataManagementPlanId", DbType.Int32));
        }

        public override void Down()
        {
            Database.ExecuteNonQuery("ALTER TABLE [Project] DROP COLUMN DataManagementPlanId");

            Database.AddColumn("[Project]", new Column("DataManagementPlanId", DbType.Int32, ColumnProperty.NotNull));
        }

    }
}
