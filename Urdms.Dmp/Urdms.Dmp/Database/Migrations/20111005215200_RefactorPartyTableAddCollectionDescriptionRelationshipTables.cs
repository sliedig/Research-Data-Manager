using System.Data;
using Curtin.Framework.Database.Migrations;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20111005215200)]
    public class RefactorPartyTableAddCollectionDescriptionRelationshipTables : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.AddColumn("[CollectionDescription]", new Column("ResearchDataDescription", DbType.String, DbString.MaxLength));
            Database.AddTable("[CollectionDescriptionRelationship]",
            new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
            new Column("Name", DbType.String, 100, ColumnProperty.NotNull));
            Database.AddTable("[PartiesToCollectionDescriptionRelationships]",
              new Column("PartyId", DbType.Int32, ColumnProperty.NotNull),
              new Column("CollectionDescriptionRelationshipId", DbType.Int32, ColumnProperty.NotNull));
        }

        public override void Down()
        {
            Database.RemoveTable("[CollectionDescriptionRelationship]");
            Database.RemoveTable("[PartiesToCollectionDescriptionRelationships]");
            Database.ExecuteNonQuery("ALTER TABLE [CollectionDescription] DROP COLUMN [ResearchDataDescription]");
        }
    }
}
