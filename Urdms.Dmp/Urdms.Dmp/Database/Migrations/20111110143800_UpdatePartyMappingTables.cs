using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20111110143800)]
    public class UpdatePartyMappingTables : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.ExecuteNonQuery("ALTER TABLE [DataCollectionParty] ALTER COLUMN [DataCollectionId] INT NULL");
            Database.ExecuteNonQuery("ALTER TABLE [ProjectParty] ALTER COLUMN [ProjectId] INT NULL");
        }

        public override void Down()
        {
            Database.ExecuteNonQuery("ALTER TABLE [DataCollectionParty] ALTER COLUMN [DataCollectionId] INT NOT NULL");
            Database.ExecuteNonQuery("ALTER TABLE [ProjectParty] ALTER COLUMN [ProjectId] INT NOT NULL");
        }
    }
}
