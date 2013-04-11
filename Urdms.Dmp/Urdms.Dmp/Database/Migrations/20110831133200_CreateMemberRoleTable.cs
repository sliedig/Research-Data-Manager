using System.Data;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20110831133200)]
    public class CreateMemberRoleTable : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.AddTable("[MemberRole]",
               new Column("MemberId", DbType.Int32, ColumnProperty.NotNull),
               new Column("RoleId", DbType.Int32, ColumnProperty.NotNull));
        }

        public override void Down()
        {
            Database.RemoveTable("[MemberRole]"); 
        }
    }
}
