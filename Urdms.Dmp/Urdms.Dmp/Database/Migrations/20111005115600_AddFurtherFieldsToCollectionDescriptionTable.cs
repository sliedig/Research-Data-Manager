using System.Data;
using Curtin.Framework.Database.Migrations;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20111005115600)]
    public class AddFurtherFieldsToCollectionDescriptionTable : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.AddColumn("[CollectionDescription]", new Column("ShareAccess", DbType.Int32));
            Database.AddColumn("[CollectionDescription]", new Column("ShareAccessDescription", DbType.String, DbString.MaxLength));
            Database.AddColumn("[CollectionDescription]", new Column("RecordCreationDate", DbType.DateTime));
            Database.ExecuteNonQuery("ALTER TABLE [CollectionDescription] DROP COLUMN [DataAccessRights]");
        }

        public override void Down()
        {
            Database.ExecuteNonQuery("ALTER TABLE [CollectionDescription] DROP COLUMN [ShareAccess]");
            Database.ExecuteNonQuery("ALTER TABLE [CollectionDescription] DROP COLUMN [ShareAccessDescription]");
            Database.ExecuteNonQuery("ALTER TABLE [CollectionDescription] DROP COLUMN [RecordCreationDate]");
            Database.AddColumn("[CollectionDescription]", new Column("DataAccessRights", DbType.Int32));
        }
    }
}
