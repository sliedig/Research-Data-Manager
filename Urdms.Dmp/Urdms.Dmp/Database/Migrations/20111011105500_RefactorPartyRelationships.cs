using System.Data;
using Curtin.Framework.Database.Migrations;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20111011105500)]
    public class RefactorPartyRelationships : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.RemoveTable("ProjectRelationship");
            Database.RemoveTable("PartiesToProjectRelationships");

            Database.RemoveTable("CollectionDescriptionRelationship");
            Database.RemoveTable("CollectionDescriptionRelationshipsToParties");

            Database.RemoveTable("CollectionDescriptionKeyword");
            Database.RemoveTable("Keyword");

            Database.ExecuteNonQuery("ALTER TABLE [Party] DROP COLUMN ProjectId");
            Database.ExecuteNonQuery("ALTER TABLE [Party] DROP COLUMN AccessRole");

            Database.AddColumn("Project", "Keywords", DbType.String, DbString.MaxLength);
            Database.AddColumn("CollectionDescription", "Keywords", DbType.String, DbString.MaxLength);

            Database.AddTable("[ProjectParty]",
                              new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                              new Column("ProjectId", DbType.Int32, ColumnProperty.NotNull),
                              new Column("PartyId", DbType.Int32, ColumnProperty.NotNull),
                              new Column("Relationship", DbType.String, 10),
                              new Column("Role", DbType.String, 10));

            Database.AddTable("[CollectionDescriptionParty]",
                              new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                              new Column("CollectionDescriptionId", DbType.Int32, ColumnProperty.NotNull),
                              new Column("PartyId", DbType.Int32, ColumnProperty.NotNull),
                              new Column("Relationship", DbType.String, 10));
        }

        public override void Down()
        {
            Database.ExecuteNonQuery(
                @"CREATE TABLE [dbo].[ProjectRelationship](
                    [Id] [int] IDENTITY(1,1) NOT NULL,
                    [Name] [nvarchar](100) NOT NULL,
                PRIMARY KEY CLUSTERED 
                (
                    [Id] ASC
                )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
                ) ON [PRIMARY]");

            Database.ExecuteNonQuery(
                @"CREATE TABLE [dbo].[PartiesToProjectRelationships](
                    [PartyId] [int] NOT NULL,
                    [ProjectRelationshipId] [int] NOT NULL
                ) ON [PRIMARY]");

            Database.ExecuteNonQuery(
                @"CREATE TABLE [dbo].[CollectionDescriptionRelationship](
                      [Id] [int] IDENTITY(1,1) NOT NULL,
                      [RelationshipType] [int] NULL,
                PRIMARY KEY CLUSTERED 
                (
                      [Id] ASC
                )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
                ) ON [PRIMARY]");

            Database.ExecuteNonQuery(
                @"CREATE TABLE [dbo].[CollectionDescriptionRelationshipsToParties](
                    [PartyId] [int] NOT NULL,
                    [CollectionDescriptionRelationshipId] [int] NOT NULL
                ) ON [PRIMARY]");

            Database.ExecuteNonQuery(
                @"CREATE TABLE [dbo].[CollectionDescriptionKeyword](
                    [Id] [int] IDENTITY(1,1) NOT NULL,
                    [Value] [nvarchar](150) NOT NULL,
                    [CollectionDescriptionId] [uniqueidentifier] NULL,
                PRIMARY KEY CLUSTERED 
                (
                    [Id] ASC
                )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
                ) ON [PRIMARY]");

            Database.ExecuteNonQuery(
                @"CREATE TABLE [dbo].[Keyword](
                    [Id] [int] IDENTITY(1,1) NOT NULL,
                    [Value] [nvarchar](150) NOT NULL,
                    [ProjectId] [int] NULL,
                PRIMARY KEY CLUSTERED 
                (
                    [Id] ASC
                )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
                ) ON [PRIMARY]");

            Database.AddColumn("Party", "ProjectId", DbType.Int32);
            Database.AddColumn("Party", "AccessRole", DbType.String, DbString.MaxLength);

            Database.RemoveColumn("Project", "Keywords");
            Database.RemoveColumn("CollectionDescription", "Keywords");

            Database.RemoveTable("[ProjectParty]");

            Database.RemoveTable("[CollectionDescriptionParty]");
        }
    }
}
