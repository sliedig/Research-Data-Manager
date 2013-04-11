using System.Data;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20110831134200)]
    public class MakeProjectIdNullable : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.ExecuteNonQuery("ALTER TABLE [Member] DROP COLUMN ProjectId");
            Database.ExecuteNonQuery("ALTER TABLE [FieldOfResearch] DROP COLUMN ProjectId");
            Database.ExecuteNonQuery("ALTER TABLE [SocioEconomicResearch] DROP COLUMN ProjectId");

            Database.AddColumn("[Member]", "[ProjectId]", DbType.Int32);
            Database.AddColumn("[FieldOfResearch]", "[ProjectId]", DbType.Int32);
            Database.AddColumn("[SocioEconomicResearch]", "[ProjectId]", DbType.Int32);
        }

        public override void Down()
        {
            Database.ExecuteNonQuery("ALTER TABLE [Member] DROP COLUMN ProjectId");
            Database.ExecuteNonQuery("ALTER TABLE [FieldOfResearch] DROP COLUMN ProjectId");
            Database.ExecuteNonQuery("ALTER TABLE [SocioEconomicResearch] DROP COLUMN ProjectId");

            Database.AddColumn("[Member]", "[ProjectId]", DbType.Int32, ColumnProperty.NotNull);
            Database.AddColumn("[FieldOfResearch]", "[ProjectId]", DbType.Int32, ColumnProperty.NotNull);
            Database.AddColumn("[SocioEconomicResearch]", "[ProjectId]", DbType.Int32, ColumnProperty.NotNull);
        }
    }
}
