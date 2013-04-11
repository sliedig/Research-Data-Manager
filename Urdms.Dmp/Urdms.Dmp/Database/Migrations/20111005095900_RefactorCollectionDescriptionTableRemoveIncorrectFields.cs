using System.Data;
using Curtin.Framework.Database.Migrations;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20111005095900)]
    public class RefactorCollectionDescriptionTableRemoveIncorrectFields : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.ExecuteNonQuery("ALTER TABLE [CollectionDescription] DROP COLUMN [Keywords]");
            Database.ExecuteNonQuery("ALTER TABLE [CollectionDescription] DROP COLUMN [SocioEconomicObjectives]");
            Database.ExecuteNonQuery("ALTER TABLE [CollectionDescription] DROP COLUMN [FieldOfResearchId]");

        }

        public override void Down()
        {
            Database.AddColumn("[CollectionDescription]", new Column("Keywords", DbType.String, DbString.MaxLength));
            Database.AddColumn("[CollectionDescription]", new Column("SocioEconomicObjectives", DbType.String, 255));
            Database.AddColumn("[CollectionDescription]", new Column("FieldOfResearchId", DbType.String, 255));
        }
    }
}
