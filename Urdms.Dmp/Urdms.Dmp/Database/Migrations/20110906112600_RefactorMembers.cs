using System.Data;
using Curtin.Framework.Database.Migrations;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20110906112600)]
    public class RefactorMembers : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.RemoveTable("[Member]");
            Database.RemoveTable("[MembersToRoles]");
            Database.RemoveTable("[Role]");

            Database.AddTable("[AccessRolesToParties]",
              new Column("PartyId", DbType.Int32, ColumnProperty.NotNull),
              new Column("AccessRoleId", DbType.Int32, ColumnProperty.NotNull));

            Database.AddTable("[Party]",
                            new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                            new Column("UserId", DbType.String, 100),
                            new Column("FirstName", DbType.String, DbString.MaxLength),
                            new Column("LastName", DbType.String, DbString.MaxLength),
                            new Column("FullName", DbType.String, DbString.MaxLength),
                            new Column("Email", DbType.String, DbString.MaxLength),
                            new Column("ProjectId", DbType.Int32),
                            new Column("Organisation", DbType.String, DbString.MaxLength));

            Database.AddTable("[AccessRole]",
             new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
             new Column("Name", DbType.String, 100, ColumnProperty.NotNull));

        }

        public override void Down()
        {
            Database.RemoveTable("[Party]");
            Database.RemoveTable("[AccessRolesToParties]");
            Database.RemoveTable("[AccessRole]");
            
            Database.AddTable("[MembersToRoles]",
             new Column("MemberId", DbType.Int32, ColumnProperty.NotNull),
             new Column("RoleId", DbType.Int32, ColumnProperty.NotNull));

            Database.AddTable("[Member]",
                            new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                            new Column("UserId", DbType.String, 100),
                            new Column("FirstName", DbType.String, DbString.MaxLength),
                            new Column("LastName", DbType.String, DbString.MaxLength),
                            new Column("FullName", DbType.String, DbString.MaxLength),
                            new Column("Email", DbType.String, DbString.MaxLength),
                            new Column("ProjectId", DbType.Int32),
                            new Column("Organisation", DbType.String, DbString.MaxLength));

            Database.AddTable("[Role]",
             new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
             new Column("RoleName", DbType.String, 100, ColumnProperty.NotNull),
             new Column("DataAccessPermission", DbType.Int32, ColumnProperty.NotNull));
           
        }
    }
}
