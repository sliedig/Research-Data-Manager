using System.Data;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20110908135700)]
    public class CreateProjectFunderTable : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.AddTable("[ProjectFunder]",
                              new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                              new Column("Funder", DbType.Int32, ColumnProperty.NotNull),
                              new Column("GrantNumber", DbType.String, 100),
                              new Column("ProjectId", DbType.Int32));
        }

        public override void Down()
        {
            Database.RemoveTable("[ProjectFunder]");
        }
    }
}
