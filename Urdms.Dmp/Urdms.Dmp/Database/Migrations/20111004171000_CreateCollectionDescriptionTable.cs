using System.Data;
using Curtin.Framework.Database.Migrations;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20111004171000)]
    public class CreateCollectionDescriptionTable : MigratorDotNetMigration
    {
        public override void Up()
        {

            Database.AddTable("[CollectionDescription]",
                              new Column("Id", DbType.Guid),
                              new Column("Title", DbType.String, DbString.MaxLength),
                              new Column("Description", DbType.String, DbString.MaxLength),
                              new Column("Type", DbType.Int32),
                              new Column("StartDate", DbType.DateTime),
                              new Column("EndDate", DbType.DateTime),
                              new Column("DataLicensingRights", DbType.Int32),
                              new Column("DataAccessRights", DbType.Int32),
                              new Column("Keywords", DbType.String, DbString.MaxLength),
                              new Column("SocioEconomicObjectives", DbType.String, 255),
                              new Column("FieldOfResearchId", DbType.String, 255));

            Database.AddTable("[PublishingRule]",
               new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
               new Column("PublishToAnds", DbType.Boolean),
               new Column("AwareOfEthics", DbType.Boolean),
               new Column("Availability", DbType.Int32),
               new Column("AvailabilityDate", DbType.DateTime));

            Database.AddTable("[CollectionDescriptionRelationship]",
            new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
            new Column("Name", DbType.String, 100, ColumnProperty.NotNull));

            Database.AddTable("[PartiesToCollectionDescriptionRelationships]",
              new Column("PartyId", DbType.Int32, ColumnProperty.NotNull),
              new Column("CollectionDescriptionRelationshipId", DbType.Int32, ColumnProperty.NotNull));

        }

        public override void Down()
        {
            Database.RemoveTable("[CollectionDescription]");
            Database.RemoveTable("[PublishingRule]");
            Database.RemoveTable("[CollectionDescriptionRelationship]");
            Database.RemoveTable("[PartiesToCollectionDescriptionRelationships]");
        }
    }
}
