using System;
using Curtin.Framework.Common.Extensions;
using Curtin.Framework.Database.NHibernate;
using Curtin.Framework.Database.NHibernate.Conventions;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;
using NHibernate;
using Urdms.Dmp.Database.Entities;

namespace Urdms.Dmp.Config.NHibernate
{
    public class NHibernateConfiguration : INHibernateConfiguration
    {
        private string _connectionString;

        public NHibernateConfiguration()
        { }

        public NHibernateConfiguration(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void SetConnectionString(string connectionString)
        {
            _connectionString = connectionString;
        }

        public ISessionFactory GetSessionFactory()
        {
#if DEBUG
                //NHibernateProfiler.Initialize();
#endif
            var cfg = new UrdmsDmpAutomappingConfig();
            var databaseConfig = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2008.ConnectionString(_connectionString))
                .Mappings(m => m.AutoMappings
                    .Add(AutoMap.AssemblyOf<NHibernateConfiguration>(cfg)
                        .UseOverridesFromAssemblyOf<NHibernateConfiguration>()
                        .Conventions.AddFromAssemblyOf<NHibernateConfiguration>()
                        .Conventions.Add<ForeignKeyNameConvention>()
                        .Conventions.Add<EnumConvention>()
                        .IgnoreBase<ClassificationBase>()
                        .IgnoreBase<Code>()
                    )
                
                );

            return databaseConfig.BuildSessionFactory();
        }
    }

    public class UrdmsDmpAutomappingConfig : DefaultAutomappingConfiguration
    {
        public override bool ShouldMap(Type type)
        {
            return (type.Namespace.NullSafe().EndsWith("Database.Entities")
                || type.Namespace.NullSafe().EndsWith("Database.Entities.Components"))
                && !type.Name.StartsWith("<");
        }

        public override bool IsComponent(Type type)
        {
            return type.Namespace.NullSafe().EndsWith("Database.Entities.Components");
        }

        // Prevent prefixing of components
        public override string GetComponentColumnPrefix(FluentNHibernate.Member member)
        {
            return string.Empty;
        }
    }

    public class ProjectOverride : IAutoMappingOverride<Project>
    {
        public void Override(AutoMapping<Project> mapping)
        {
            mapping.HasMany(x => x.Parties).Inverse().Cascade.AllDeleteOrphan();
            mapping.HasMany(x => x.SocioEconomicObjectives).Cascade.All();
            mapping.HasMany(x => x.FieldsOfResearch).Cascade.All();
            mapping.HasMany(x => x.Funders).Cascade.AllDeleteOrphan();
            mapping.References(x => x.DataManagementPlan).ForeignKey("DataManagementPlanId").Not.LazyLoad().Fetch.Join().Cascade.SaveUpdate();
            mapping.References(x => x.DataDeposit).ForeignKey("DataDepositId").Not.LazyLoad().Fetch.Join().Cascade.SaveUpdate();
            mapping.HasMany(x => x.DataCollections).Cascade.All();
            mapping.Map(x => x.ProvisioningStatus).ReadOnly();
        }
    }

    public class ProjectPartyOverride : IAutoMappingOverride<ProjectParty>
    {
        public void Override(AutoMapping<ProjectParty> mapping)
        {
            mapping.References(x => x.Project).ForeignKey("ProjectId").Cascade.SaveUpdate();
            mapping.References(x => x.Party).ForeignKey("PartyId").Not.LazyLoad().Fetch.Join().Cascade.SaveUpdate();
        }
    }

    public class ProjectFieldOfResearchOverride : IAutoMappingOverride<ProjectFieldOfResearch>
    {
        public void Override(AutoMapping<ProjectFieldOfResearch> mapping)
        {
            mapping.References(x => x.FieldOfResearch).ForeignKey("FieldOfResearchId").Not.LazyLoad().Fetch.Join().Cascade.SaveUpdate();
            mapping.IgnoreProperty(x => x.Code);
        }
    }

    public class ProjectSocioEconomicObjectiveOverride : IAutoMappingOverride<ProjectSocioEconomicObjective>
    {
        public void Override(AutoMapping<ProjectSocioEconomicObjective> mapping)
        {
            mapping.References(x => x.SocioEconomicObjective).ForeignKey("SocioEconomicObjectiveId").Not.LazyLoad().Fetch.Join().Cascade.SaveUpdate();
            mapping.IgnoreProperty(x => x.Code);
        }
    }

    public class DataCollectionOverride : IAutoMappingOverride<DataCollection>
    {
        public void Override(AutoMapping<DataCollection> mapping)
        {
            mapping.HasMany(x => x.Parties).Inverse().Cascade.AllDeleteOrphan();
            mapping.HasMany(x => x.SocioEconomicObjectives).Cascade.All();
            mapping.HasMany(x => x.FieldsOfResearch).Cascade.All();
        }
    }

    public class DataCollectionPartyOverride : IAutoMappingOverride<DataCollectionParty>
    {
        public void Override(AutoMapping<DataCollectionParty> mapping)
        {
            mapping.References(x => x.DataCollection).ForeignKey("DataCollectionId").Cascade.SaveUpdate();
            mapping.References(x => x.Party).ForeignKey("PartyId").Not.LazyLoad().Fetch.Join().Cascade.SaveUpdate();
        }
    }

    public class DataCollectionFieldOfResearchOverride : IAutoMappingOverride<DataCollectionFieldOfResearch>
    {
        public void Override(AutoMapping<DataCollectionFieldOfResearch> mapping)
        {
            mapping.References(x => x.FieldOfResearch).ForeignKey("FieldOfResearchId").Not.LazyLoad().Fetch.Join().Cascade.SaveUpdate();
            mapping.IgnoreProperty(x => x.Code);
        }
    }

    public class DataCollectionSocioEconomicObjectiveOverride : IAutoMappingOverride<DataCollectionSocioEconomicObjective>
    {
        public void Override(AutoMapping<DataCollectionSocioEconomicObjective> mapping)
        {
            mapping.References(x => x.SocioEconomicObjective).ForeignKey("SocioEconomicObjectiveId").Not.LazyLoad().Fetch.Join().Cascade.SaveUpdate();
            mapping.IgnoreProperty(x => x.Code);
        }
    }

    public class FormTimerOverride : IAutoMappingOverride<FormTimer>
    {
        public void Override(AutoMapping<FormTimer> mapping)
        {
            mapping.Id(x => x.Id).GeneratedBy.Assigned();
            mapping.Id(x => x.Step).GeneratedBy.Assigned();

            mapping.CompositeId()
                .KeyProperty(x => x.Id)
                .KeyProperty(x => x.Step);
        }
    }

    public class DefaultStringLengthConvention : IPropertyConvention, IPropertyConventionAcceptance
    {
        public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria)
        {
            // For any strings where a length has not been specified
            criteria.Expect(x => x.Type == typeof(string)).Expect(x => x.Length == 0);
        }

        public void Apply(IPropertyInstance instance)
        {
            instance.CustomType("StringClob");
            instance.CustomSqlType("text");
        }
    }
}
