using System.Data;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20111005135700)]
    public class RefactorCollectionDescriptionTableMergePublishingRules : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.AddColumn("[CollectionDescription]", new Column("PublishToAnds", DbType.Boolean));
            Database.AddColumn("[CollectionDescription]", new Column("AwareOfEthics", DbType.Boolean));
            Database.AddColumn("[CollectionDescription]", new Column("Availability", DbType.Int32));
            Database.AddColumn("[CollectionDescription]", new Column("AvailabilityDate", DbType.DateTime));
        }

        public override void Down()
        {
            Database.ExecuteNonQuery("ALTER TABLE [CollectionDescription] DROP COLUMN [PublishToAnds]");
            Database.ExecuteNonQuery("ALTER TABLE [CollectionDescription] DROP COLUMN [AwareOfEthics]");
            Database.ExecuteNonQuery("ALTER TABLE [CollectionDescription] DROP COLUMN [Availability]");
            Database.ExecuteNonQuery("ALTER TABLE [CollectionDescription] DROP COLUMN [AvailabilityDate]");
        }
    }
}
