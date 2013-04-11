using System.Data;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20111013160700)]
    public class RefactorCollectionDescriptionIdToInt : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.RemoveTable("[CollectionDescription]");
            Database.ExecuteNonQuery(@"CREATE TABLE [dbo].[CollectionDescription](
                                            [Title] [nvarchar](max) NULL,
                                            [Type] [int] NULL,
                                            [StartDate] [datetime] NULL,
                                            [EndDate] [datetime] NULL,
                                            [DataLicensingRights] [int] NULL,
                                            [ProjectId] [int] NULL,
                                            [ShareAccess] [int] NULL,
                                            [ShareAccessDescription] [nvarchar](max) NULL,
                                            [RecordCreationDate] [datetime] NULL,
                                            [PublishToAnds] [bit] NULL,
                                            [AwareOfEthics] [bit] NULL,
                                            [Availability] [int] NULL,
                                            [AvailabilityDate] [datetime] NULL,
                                            [ResearchDataDescription] [nvarchar](max) NULL,
                                            [ResearchDataProcess] [nvarchar](max) NULL,
                                            [Keywords] [nvarchar](max) NULL,
                                            [Id] [int] IDENTITY(1,1) NOT NULL,
                                        PRIMARY KEY CLUSTERED 
                                        (
                                            [Id] ASC
                                        )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
                                        ) ON [PRIMARY]");


            Database.ExecuteNonQuery("ALTER TABLE [CollectionDescriptionSocioEconomicObjective] DROP COLUMN CollectionDescriptionId");
            Database.ExecuteNonQuery("ALTER TABLE [CollectionDescriptionFieldOfResearch] DROP COLUMN CollectionDescriptionId");
            Database.ExecuteNonQuery("ALTER TABLE [CollectionDescriptionApprover] DROP COLUMN CollectionDescriptionId");
            Database.AddColumn("[CollectionDescriptionSocioEconomicObjective]", new Column("CollectionDescriptionId", DbType.Int32));
            Database.AddColumn("[CollectionDescriptionFieldOfResearch]", new Column("CollectionDescriptionId", DbType.Int32));
            Database.AddColumn("[CollectionDescriptionApprover]", new Column("CollectionDescriptionId", DbType.Int32));
        }

        public override void Down()
        {
            Database.RemoveTable("[CollectionDescription]");
            Database.ExecuteNonQuery(@"CREATE TABLE [dbo].[CollectionDescription](
                                            [Title] [nvarchar](max) NULL,
                                            [Type] [int] NULL,
                                            [StartDate] [datetime] NULL,
                                            [EndDate] [datetime] NULL,
                                            [DataLicensingRights] [int] NULL,
                                            [ProjectId] [int] NULL,
                                            [ShareAccess] [int] NULL,
                                            [ShareAccessDescription] [nvarchar](max) NULL,
                                            [RecordCreationDate] [datetime] NULL,
                                            [PublishToAnds] [bit] NULL,
                                            [AwareOfEthics] [bit] NULL,
                                            [Availability] [int] NULL,
                                            [AvailabilityDate] [datetime] NULL,
                                            [ResearchDataDescription] [nvarchar](max) NULL,
                                            [ResearchDataProcess] [nvarchar](max) NULL,
                                            [Keywords] [nvarchar](max) NULL,
                                            [Id] [uniqueidentifier] NOT NULL,
                                            CONSTRAINT [PK_CollectionDescription] PRIMARY KEY CLUSTERED 
                                        (
                                            [Id] ASC
                                        )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
                                        ) ON [PRIMARY]
                                        ALTER TABLE [dbo].[CollectionDescription] ADD  CONSTRAINT [DF_CollectionDescription_Id]  DEFAULT (newid()) FOR [Id]"); 
            
            Database.ExecuteNonQuery("ALTER TABLE [CollectionDescriptionSocioEconomicObjective] DROP COLUMN CollectionDescriptionId");
            Database.ExecuteNonQuery("ALTER TABLE [CollectionDescriptionFieldOfResearch] DROP COLUMN CollectionDescriptionId");
            Database.ExecuteNonQuery("ALTER TABLE [CollectionDescriptionApprover] DROP COLUMN CollectionDescriptionId");
            Database.AddColumn("[CollectionDescriptionSocioEconomicObjective]", new Column("CollectionDescriptionId", DbType.Guid));
            Database.AddColumn("[CollectionDescriptionFieldOfResearch]", new Column("CollectionDescriptionId", DbType.Guid));
            Database.AddColumn("[CollectionDescriptionApprover]", new Column("CollectionDescriptionId", DbType.Guid));
        }
      
    }
}
