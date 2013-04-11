using System.Data;
using Curtin.Framework.Database.Migrations;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20110815140600)]
    public class CreateIntellectualPropertyTable : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.AddTable("[IntellectualProperty]",
                new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                new Column("IsCopyrighted", DbType.Boolean, ColumnProperty.NotNull),
                new Column("CopyrightOwners", DbType.String, DbString.MaxLength),
                new Column("DmpId", DbType.Int32, ColumnProperty.NotNull));
        }

        public override void Down()
        {
            Database.RemoveTable("[IntellectualProperty]");
        }
    }
}
