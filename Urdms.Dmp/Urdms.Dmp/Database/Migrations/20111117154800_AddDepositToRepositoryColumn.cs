using System.Data;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20111117154800)]
    public class AddDepositToRepositoryColumn : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.AddColumn("DataManagementPlan", "DataRetentionDepositToRepository", DbType.Boolean);
        }

        public override void Down()
        {
            Database.ExecuteNonQuery("ALTER TABLE DataManagementPlan DROP COLUMN DataRetentionDepositToRepository");
        }
    }
}
