using System.Data;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20110831134700)]
    public class CreateMembersToRolesTable : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.RemoveTable("[MemberRole]");

            Database.AddTable("[MembersToRoles]",
               new Column("MemberId", DbType.Int32, ColumnProperty.NotNull),
               new Column("RoleId", DbType.Int32, ColumnProperty.NotNull));
        }

        public override void Down()
        {
            Database.RemoveTable("[MembersToRoles]");

            Database.AddTable("[MemberRole]",
              new Column("MemberId", DbType.Int32, ColumnProperty.NotNull),
              new Column("RoleId", DbType.Int32, ColumnProperty.NotNull));
        }
    }
}
