using System.Data;
using Curtin.Framework.Database.Migrations;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20111122153200)]
    public class DataCollectionChanges : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.ExecuteNonQuery("ALTER TABLE DataCollection DROP COLUMN PublishToAnds");
            Database.ExecuteNonQuery("ALTER TABLE DataCollection DROP COLUMN ResearchDataProcess");

            Database.AddColumn("DataCollection", "EthicsApprovalNumber", DbType.String, 255);
            Database.AddColumn("DataCollection", "DataStoreLocationName", DbType.String);
            Database.AddColumn("DataCollection", "DataStoreLocationUrl", DbType.String);
            Database.AddColumn("DataCollection", "DataStoreAdditionalDetails", DbType.String);
            Database.AddColumn("DataCollection", "DataCollectionIdentifier", DbType.Int32);
            Database.AddColumn("DataCollection", "DataCollectionIdentifierValue", DbType.String);
        }

        public override void Down()
        {
            Database.AddColumn("DataCollection", "PublishToAnds", DbType.Boolean);
            Database.AddColumn("DataCollection", "ResearchDataProcess", DbType.String, DbString.MaxLength);

            Database.ExecuteNonQuery("ALTER TABLE DataCollection DROP COLUMN EthicsApprovalNumber");
            Database.ExecuteNonQuery("ALTER TABLE DataCollection DROP COLUMN DataStoreLocationName");
            Database.ExecuteNonQuery("ALTER TABLE DataCollection DROP COLUMN DataStoreLocationUrl");
            Database.ExecuteNonQuery("ALTER TABLE DataCollection DROP COLUMN DataStoreAdditionalDetails");
            Database.ExecuteNonQuery("ALTER TABLE DataCollection DROP COLUMN DataCollectionIdentifier");
            Database.ExecuteNonQuery("ALTER TABLE DataCollection DROP COLUMN DataCollectionIdentifierValue");
        }
    }
}
