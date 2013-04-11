using System.Data;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20111018170800)]
    public class AddStatusToDataCollection : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.AddColumn("DataCollection", "Status", DbType.Int32);
            Database.ExecuteNonQuery("ALTER TABLE [DataCollectionParty] DROP COLUMN [Relationship]");
            Database.AddColumn("DataCollectionParty", "Relationship", DbType.Int32);
            Database.ExecuteNonQuery("ALTER TABLE [ProjectParty] DROP COLUMN [Role]");
            Database.AddColumn("ProjectParty", "Role", DbType.Int32);
            Database.ExecuteNonQuery("ALTER TABLE [ProjectParty] DROP COLUMN [Relationship]");
            Database.AddColumn("ProjectParty", "Relationship", DbType.Int32);

        }

        public override void Down()
        {
            Database.ExecuteNonQuery("ALTER TABLE [DataCollection] DROP COLUMN [Status]");
            Database.ExecuteNonQuery("ALTER TABLE [DataCollectionParty] DROP COLUMN [Relationship]");
            Database.AddColumn("DataCollectionParty", "Relationship", DbType.String, 10);
            Database.ExecuteNonQuery("ALTER TABLE [ProjectParty] DROP COLUMN [Role]");
            Database.AddColumn("ProjectParty", "Role", DbType.String, 10);
            Database.ExecuteNonQuery("ALTER TABLE [ProjectParty] DROP COLUMN [Relationship]");
            Database.AddColumn("ProjectParty", "Relationship", DbType.String, 10);
        }
    }
}
