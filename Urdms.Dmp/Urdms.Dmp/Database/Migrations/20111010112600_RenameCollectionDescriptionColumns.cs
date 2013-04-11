using System.Data;
using Curtin.Framework.Database.Migrations;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20111010112600)]
    public class RenameCollectionDescriptionColumns : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.ExecuteNonQuery("ALTER TABLE [CollectionDescription] DROP COLUMN [ResearchDataDescription]");
            Database.ExecuteNonQuery("ALTER TABLE [CollectionDescription] DROP COLUMN [Description]");
            Database.ExecuteNonQuery("ALTER TABLE [CollectionDescriptionRelationship] DROP COLUMN [Name]");
            Database.AddColumn("[CollectionDescription]", new Column("ResearchDataDescription", DbType.String, DbString.MaxLength));
            Database.AddColumn("[CollectionDescription]", new Column("ResearchDataProcess", DbType.String, DbString.MaxLength));
            Database.AddColumn("[CollectionDescriptionRelationship]", new Column("RelationshipType", DbType.Int32));
        }

        public override void Down()
        {
            Database.ExecuteNonQuery("ALTER TABLE [CollectionDescription] DROP COLUMN [ResearchDataDescription]");
            Database.ExecuteNonQuery("ALTER TABLE [CollectionDescription] DROP COLUMN [ResearchDataProcess]");
            Database.ExecuteNonQuery("ALTER TABLE [CollectionDescriptionRelationship] DROP COLUMN [RelationshipType]");
            Database.AddColumn("[CollectionDescription]", new Column("Description", DbType.String, DbString.MaxLength));
            Database.AddColumn("[CollectionDescription]", new Column("ResearchDataDescription", DbType.String, DbString.MaxLength));
            Database.AddColumn("[CollectionDescriptionRelationship]", new Column("Name", DbType.String, DbString.MaxLength));
        }
    }
}
