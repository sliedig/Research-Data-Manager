using System.Data;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20111118101600)]
    public class RemoveBackupFrequencyColumn : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.ExecuteNonQuery("ALTER TABLE DataManagementPlan DROP COLUMN BackupPolicyFrequency");
        }

        public override void Down()
        {
            Database.AddColumn("DataManagementPlan", "BackupPolicyFrequency", DbType.Int32);
        }
    }
}
