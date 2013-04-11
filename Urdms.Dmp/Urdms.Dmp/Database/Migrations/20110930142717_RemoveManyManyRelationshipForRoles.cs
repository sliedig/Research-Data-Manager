using System.Data;
using Curtin.Framework.Database.Migrations;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20110930142717)]
    public class RemoveManyManyRelationshipForRoles : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.RemoveTable("[AccessRolesToParties]");
            Database.RemoveTable("[AccessRole]");
            
            Database.AddColumn("[Party]",
                new Column("AccessRole", DbType.String, DbString.MaxLength));
        }

        public override void Down()
        {
            Database.AddTable("[AccessRolesToParties]",
              new Column("PartyId", DbType.Int32, ColumnProperty.NotNull),
              new Column("AccessRoleId", DbType.Int32, ColumnProperty.NotNull));

            Database.AddTable("[AccessRole]",
             new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
             new Column("Name", DbType.String, 100, ColumnProperty.NotNull));
        }
    }
}
