using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Curtin.Framework.Common.Extensions;
using Omu.ValueInjecter;
using Urdms.Dmp.Database.Entities;
using Urdms.Dmp.Models;
using Urdms.Dmp.Models.Enums;
using Urdms.Dmp.Utils;

namespace Urdms.Dmp.Mappers
{
    public static class ProjectMapper
    {
        public static Project MapFrom(this Project entity, ProjectViewModel model, bool removeMissingParties = false)
        {
            if (model == null || entity == null)
                return entity;

            entity.InjectFrom<SameNameWithRecursionAndTargetInstantiationIfRequired>(model);
            IList<int> projectPartyIds = null;
            if (entity.SourceProjectType == SourceProjectType.DEPOSIT)
            {
                entity.Parties.MapFrom(model.DataDeposit);
                if (removeMissingParties)
                {
                    // combining project party ids of all parties from the view model's data deposit
                    projectPartyIds = model.DataDeposit.UrdmsUsers
                        .Where(o => o.Id > 0)
                        .Select(o => o.Id)
                        .Union(model.DataDeposit.NonUrdmsUsers
                                   .Where(o => o.Id > 0)
                                   .Select(o => o.Id))
                        .ToList();
                    
                }
            }
            else
            {
                entity.Parties.MapFrom(model.DataManagementPlan);
                if (removeMissingParties)
                {
                    // combining project party ids of all parties from the view model's dmp
                    projectPartyIds = model.DataManagementPlan.UrdmsUsers
                        .Where(o => o.Id > 0)
                        .Select(o => o.Id)
                        .Union(model.DataManagementPlan.NonUrdmsUsers
                                   .Where(o => o.Id > 0)
                                   .Select(o => o.Id))
                        .ToList();
                } 
            }
            if (projectPartyIds != null && projectPartyIds.Count != 0)
            {
                // Get a list of ProjectParty keys requiring deletion
                var partyDeleteKeys = entity.Parties
                    .Where(o => o.Relationship != ProjectRelationship.PrincipalInvestigator && o.Id > 0 && !projectPartyIds.Any(q => o.Id == q))
                    .Select(t => t.Id).ToList();

                // Remove them
                partyDeleteKeys.Do(o => entity.Parties.RemoveAll(p => p.Id == o));
            }
            entity.Parties.ForEach(p => p.Project = entity);

            return entity;
        }


        public static ProjectViewModel MapFrom(this ProjectViewModel model, Project entity, bool mapDmp = true, bool mapDd = true)
        {
            if (model == null || entity == null)
            {
                return model;
            }
            
            model.InjectFrom(entity);
            if (entity.StartDate != null)
            {
                model.StartDate = entity.StartDate.Value.ToShortDateString();
            }
            if (entity.EndDate != null)
            {
                model.EndDate = entity.EndDate.Value.ToShortDateString();
            }
            model.PrincipalInvestigator = entity.Parties
                                            .Where(p => !string.IsNullOrWhiteSpace(p.Party.UserId) && p.Relationship == ProjectRelationship.PrincipalInvestigator)
                                            .Single()
                                            .Party;

            model.FieldsOfResearch.AddRange(entity.FieldsOfResearch);
            model.SocioEconomicObjectives.AddRange(entity.SocioEconomicObjectives);
           
            if (model.DataManagementPlan != null && mapDmp)
            {
                model.DataManagementPlan.MapFrom(entity);
            }

            if (model.DataDeposit != null && mapDd)
            {
                model.DataDeposit.MapFrom(entity);
            }
            return model;
        }

        public static ProjectReadOnlyDetailsViewModel MapFrom(this ProjectReadOnlyDetailsViewModel model, Project entity)
        {
            if (model == null || entity == null)
            {
                return model;
            }
            model.InjectFrom(entity);
            model.FieldsOfResearch.AddRange(entity.FieldsOfResearch);
            model.SocioEconomicObjectives.AddRange(entity.SocioEconomicObjectives);
            model.PrincipalInvestigator = entity.Parties.Single(o => o.Relationship == ProjectRelationship.PrincipalInvestigator).Party;
            return model;
        }

        public static Project MapFrom(this Project entity, ProjectDetailsViewModel model)
        {
            if (model == null || entity == null)
            {
                return entity;
            }
            
            DateTime startDate;
            if (string.IsNullOrWhiteSpace(model.StartDate) || !DateTime.TryParse(model.StartDate, out startDate))
            {
                entity.StartDate = null;
            }
            else
            {
                entity.StartDate = startDate.Date;
            }

            DateTime endDate;
            if (string.IsNullOrWhiteSpace(model.EndDate) || !DateTime.TryParse(model.EndDate, out endDate))
            {
                entity.EndDate = null;
            }
            else
            {
                entity.EndDate = endDate.Date;
            }

            entity.Funders.MapFrom(model.Funders);
            entity.FieldsOfResearch.MapFrom<ProjectFieldOfResearch, FieldOfResearch>(model.FieldsOfResearch.Select(s => (ProjectFieldOfResearch) s).ToList());
            entity.SocioEconomicObjectives.MapFrom<ProjectSocioEconomicObjective, SocioEconomicObjective>(model.SocioEconomicObjectives.Select(s => (ProjectSocioEconomicObjective)s).ToList());

            return entity;
        }

        public static ProjectDetailsViewModel MapFrom(this ProjectDetailsViewModel model, Project entity)
        {
            if (entity == null || model == null)
            {
                return model;
            }

            model.InjectFrom<SameNameWithRecursion>(entity);
            if (entity.StartDate != null)
            {
                model.StartDate = entity.StartDate.Value.ToShortDateString();
            }
            if (entity.EndDate != null)
            {
                model.EndDate = entity.EndDate.Value.ToShortDateString();
            }
            if (entity.Funders.IsNotEmpty())
            {
                var funderTypes = Enum.GetValues(typeof(Funder)).Cast<Funder>().Where(o => entity.Funders.Any(q => q.Funder == o)).ToList();

                foreach (var funderType in funderTypes)
                {
                    var viewModel = model.Funders.FirstOrDefault(o => o.Funder == funderType);

                    if (viewModel == null) continue;

                    var funder = entity.Funders.First(o => o.Funder == funderType);

                    viewModel.IsFunded = true;
                    viewModel.GrantNumber = funder.GrantNumber;
                }
            }

            if (entity.SocioEconomicObjectives.IsNotEmpty())
            {
                var newObjectives = (entity.SocioEconomicObjectives.Where(o => !model.SocioEconomicObjectives
                                                                                    .Any(q => o.SocioEconomicObjective.Id == q.Code.Id))
                    .Select(o => (ProjectSocioEconomicObjective)new ProjectSocioEconomicObjective().InjectFrom(o)))
                    .ToList();

                if (newObjectives.IsNotEmpty())
                {
                    model.SocioEconomicObjectives.AddRange(newObjectives);
                }
            }

            if (!entity.FieldsOfResearch.IsNotEmpty()) return model;

            var newResearchList = (from o in entity.FieldsOfResearch
                                   where !model.FieldsOfResearch.Any(q => o.FieldOfResearch.Id == q.Code.Id)
                                   select (ProjectFieldOfResearch)new ProjectFieldOfResearch().InjectFrom(o)).ToList();
            if (newResearchList.IsNotEmpty())
            {
                model.FieldsOfResearch.AddRange(newResearchList);
            }

            return model;
        }

        public static IList<ProjectListViewModel> MapFrom(this IList<ProjectListViewModel> model, IList<Project> entity)
        {
            if (entity == null || model == null)
                return model;
            model.AddRange(
                entity.Select(o => new {o, dmp = o.DataManagementPlan ?? new DataManagementPlan(), dd = o.DataDeposit ?? new DataDeposit()}).
                    Select(@t => new ProjectListViewModel
                                     {
                                         ProjectId = t.o.Id,
                                         ProjectTitle = t.o.Title,
                                         ProjectType = t.o.SourceProjectType,
                                         ProjectStatus = t.o.Status.GetDescription(),
                                         DmpCreationDate = t.dmp.Id > 0 ? t.dmp.CreationDate.ToShortDateString() : "",
                                         ProvisioningStatus = t.o.ProvisioningStatus,
                                         SiteUrl = t.o.SiteUrl,
                                         DmpId = t.dmp.Id,
                                         DataDepositId = t.dd.Id,
                                         HasFirstCollection = t.o.DataCollections != null && t.o.DataCollections.SingleOrDefault(z => z.IsFirstCollection) != null
                                     }));

            return model;
        }

        public static IList<ProjectFunder> MapFrom(this IList<ProjectFunder> funders, IEnumerable<ProjectFunderViewModel> models)
        {
            funders.Clear();
            funders.AddRange(models.Where(o => o.IsFunded).Select(f => new ProjectFunder { Funder = f.Funder, GrantNumber = f.GrantNumber }));

            return funders;
        }

        public static IList<T> MapFrom<T, TK>(this IList<T> destination, IList<T> source)
            where T : ClassificationBase, new()
            where TK : Code, new()
        {
            if (source == null)
            {
                return destination;
            }
            if (destination == null)
            {
                destination = new List<T>(source);
            }
            if (destination.IsEmpty())
            {
                destination.AddRange(source);
                return destination;
            }

            foreach (var entity in 
                    source.Select(item => new {item, entity = destination.FirstOrDefault(o => o.Code.Id == item.Code.Id)})
                    .Where(@t => t.entity == null).Select(@t => new T
                                                                     {
                                                                         Code =
                                                                             new TK
                                                                                 {
                                                                                     Id = t.item.Code.Id,
                                                                                     Name = t.item.Code.Name
                                                                                 }
                                                                     }))
            {
                destination.Add(entity);
            }

            var deletedKeys = destination.Where(o => !source.Any(q => o.Code.Id == q.Code.Id)).Select(o => o.Code.Id);
            var deletedEntities = deletedKeys
                .Select(deleteKey => destination.FirstOrDefault(o => o.Code.Id == deleteKey))
                .Where(deletedEntity => deletedEntity != null)
                .ToList();

            foreach (var deletedEntity in deletedEntities)
            {
                destination.Remove(deletedEntity);
            }

            return destination;
        }
    }
}
