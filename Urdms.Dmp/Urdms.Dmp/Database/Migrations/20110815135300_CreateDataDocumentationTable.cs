using System.Data;
using Curtin.Framework.Database.Migrations;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20110815135300)]
    public class CreateDataDocumentationTable : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.AddTable("[DataDocumentation]",
                new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                new Column("MetadataStandards", DbType.String, DbString.MaxLength),
                new Column("DirectoryStructure", DbType.String, DbString.MaxLength),
                new Column("FileNamingConvention", DbType.String, DbString.MaxLength),
                new Column("VersionControl", DbType.Int32, ColumnProperty.NotNull),
                new Column("VersionControlDescription", DbType.String, DbString.MaxLength),
                new Column("DmpId", DbType.Int32, ColumnProperty.NotNull));
        }

        public override void Down()
        {
            Database.RemoveTable("[DataDocumentation]");
        }
    }
}
