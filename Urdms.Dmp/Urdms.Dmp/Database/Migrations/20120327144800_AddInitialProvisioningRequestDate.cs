using System.Data;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20120327144800)]
    public class AddInitialProvisioningRequestDate : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.AddColumn("[Project]", new Column("InitialProvisioningRequestDate", DbType.DateTime, ColumnProperty.Null));
        }

        public override void Down()
        {
            Database.RemoveColumn("[Project]", "InitialProvisioningRequestDate");

        }
    }
}
