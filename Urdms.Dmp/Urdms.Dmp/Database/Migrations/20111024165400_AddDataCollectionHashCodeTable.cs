using System.Data;
using Curtin.Framework.Database.Migrations;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20111024165400)]
    public class AddDataCollectionHashCodeTable : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.AddTable("[DataCollectionHashCode]",
                              new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                              new Column("DataCollectionId", DbType.Int32,ColumnProperty.NotNull),
                              new Column("HashCode", DbType.String, DbString.MaxLength, ColumnProperty.NotNull));
            
        }

        public override void Down()
        {
            Database.RemoveTable("[DataCollectionHashCode]");
        }
    }
}
