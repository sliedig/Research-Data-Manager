using System.Data;
using Curtin.Framework.Database.Migrations;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20110815130800)]
    public class CreateProjectTable : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.AddTable("[Project]",
                new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                new Column("[Key]", DbType.String, DbString.MaxLength, ColumnProperty.NotNull),
                new Column("Name", DbType.String, DbString.MaxLength, ColumnProperty.NotNull),
                new Column("Description", DbType.String, DbString.MaxLength, ColumnProperty.NotNull),
                new Column("DmpId", DbType.Int32, ColumnProperty.NotNull));
        }

        public override void Down()
        {
            Database.RemoveTable("[Project]");
        }
    }
}
