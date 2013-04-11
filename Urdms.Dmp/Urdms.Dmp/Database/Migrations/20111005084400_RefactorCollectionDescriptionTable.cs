using System.Data;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20111005084400)]
    public class RefactorCollectionDescriptionTable : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.AddColumn("[CollectionDescription]", new Column("ProjectId", DbType.Int32));
            Database.RemoveTable("[CollectionDescriptionRelationship]");
            Database.RemoveTable("[PartiesToCollectionDescriptionRelationships]");
            Database.AddTable("[CollectionDescriptionKeyword]",
             new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
             new Column("Value", DbType.String, 150, ColumnProperty.NotNull),
             new Column("CollectionDescriptionId", DbType.Int32));
            Database.AddTable("[CollectionDescriptionSocioEconomicObjective]",
                              new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                              new Column("CollectionDescriptionId", DbType.Int32),
                              new Column("SocioEconomicObjectiveId", DbType.Int32));
            Database.AddTable("[CollectionDescriptionFieldOfResearch]",
                             new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                             new Column("CollectionDescriptionId", DbType.Int32),
                             new Column("FieldOfResearchId", DbType.Int32));
            Database.AddTable("[CollectionDescriptionApprover]",
                              new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                              new Column("CollectionDescriptionId", DbType.Int32),
                              new Column("ApprovalType", DbType.Int32),
                              new Column("ApproverId", DbType.String, 100, ColumnProperty.NotNull),
                              new Column("ApproverName", DbType.String, 100, ColumnProperty.NotNull),
                              new Column("ApprovedOn", DbType.DateTime, ColumnProperty.NotNull));

        }

        public override void Down()
        {
            Database.ExecuteNonQuery("ALTER TABLE [CollectionDescription] DROP COLUMN [ProjectId]");
            Database.AddTable("[CollectionDescriptionRelationship]",
              new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
              new Column("Name", DbType.String, 100, ColumnProperty.NotNull));
            Database.AddTable("[PartiesToCollectionDescriptionRelationships]",
              new Column("PartyId", DbType.Int32, ColumnProperty.NotNull),
              new Column("CollectionDescriptionRelationshipId", DbType.Int32, ColumnProperty.NotNull));
            Database.RemoveTable("[CollectionDescriptionKeyword]");
            Database.RemoveTable("[CollectionDescriptionSocioEconomicObjective]");
            Database.RemoveTable("[CollectionDescriptionFieldOfResearch]");
            Database.RemoveTable("[CollectionDescriptionApprover]");
        }
    }
}
