using System.Collections.Generic;
using System.Linq;
using Curtin.Framework.Common.Extensions;
using Omu.ValueInjecter;
using Urdms.Dmp.Database.Entities;
using Urdms.Dmp.Models;
using Urdms.Dmp.Models.DataCollectionModels;
using Urdms.Dmp.Models.Enums;
using Urdms.Dmp.Utils;

namespace Urdms.Dmp.Mappers
{
    public static class DataCollectionMapper
    {
        public static DataCollection MapFrom(this DataCollection entity, DataCollectionViewModelStep1 vm)
        {
            if (vm == null || entity == null)
                return entity;

            string[] exclusions = entity.IsFirstCollection ? new[] {"DataStoreLocationName", "DataStoreLocationUrl", "IsFirstCollection"} : new string[] {};
            var injection = new SameNameWithRecursion(exclusions);
            entity.InjectFrom(injection, vm);
            return entity;
        }

        public static DataCollection MapFrom(this DataCollection entity, DataCollectionViewModelStep2 vm, bool removeMissingParties = false)
        {
            if (vm == null || entity == null)
                return entity;

            
            entity.InjectFrom(vm);
           

            entity.FieldsOfResearch.MapFrom<DataCollectionFieldOfResearch, FieldOfResearch>(vm.FieldsOfResearch.Select(s => (DataCollectionFieldOfResearch)s).ToList());
            entity.SocioEconomicObjectives.MapFrom<DataCollectionSocioEconomicObjective, SocioEconomicObjective>(vm.SocioEconomicObjectives.Select(s => (DataCollectionSocioEconomicObjective) s).ToList());

            
            entity.Parties.MapFrom(vm);
            if (removeMissingParties)
            {
				// combining project party ids of all parties from the view model URDMS and non URDMS users
                var dataCollectionPartyIds = vm.UrdmsUsers
                    .Where(o => o.Id > 0)
                    .Select(o => o.Id)
                    .Union(vm.NonUrdmsUsers.Where(o => o.Id > 0)
                               .Select(o => o.Id))
                    .ToList();
                if (dataCollectionPartyIds.Count != 0)
                {
                    // Get a list of ProjectParty keys requiring deletion
                    var partyDeleteKeys = entity.Parties
                        .Where(o => o.Relationship != DataCollectionRelationshipType.Manager && o.Id > 0 && !dataCollectionPartyIds.Any(q => o.Id == q))
                        .Select(t => t.Id)
                        .ToList();

                    // Remove them
                    partyDeleteKeys.Do(o => entity.Parties.RemoveAll(p => p.Id == o));
                }
            }
            entity.Parties.ForEach(p => p.DataCollection = entity);

            return entity;
        }

        public static DataCollectionViewModelStep1 MapFrom(this DataCollectionViewModelStep1 vm, Project project, DataCollection collection = null)
        {
            if (vm == null || project == null)
            {
                return vm;
            }
            if (collection != null)
            {
                vm.InjectFrom(collection);
            }
            vm.ProjectTitle = project.Title;
            return vm;
        }

        public static DataCollectionViewModelStep2 MapFrom(this DataCollectionViewModelStep2 vm, DataCollection collection)
        {
            if (vm == null || collection == null)
                return vm;

            vm.InjectFrom(collection);

            MapForCodes(vm.FieldsOfResearch, collection.FieldsOfResearch);
            MapSeoCodes(vm.SocioEconomicObjectives, collection.SocioEconomicObjectives);
            vm.Manager = MapManager(collection.Parties);
            vm.UrdmsUsers = MapUrdmsUsers(collection.Parties);
            vm.NonUrdmsUsers = MapNonUrdmsUsers(collection.Parties);
            return vm;
        }

        public static DataCollectionReadOnlyViewModel MapFrom(this DataCollectionReadOnlyViewModel vm, DataCollection collection, Project project)
        {
            if (vm == null || collection == null || project == null)
                return vm;

            vm.InjectFrom(collection);

            MapForCodes(vm.FieldsOfResearch, collection.FieldsOfResearch);
            MapSeoCodes(vm.SocioEconomicObjectives, collection.SocioEconomicObjectives);
            vm.Manager = MapManager(collection.Parties);
            vm.UrdmsUsers = MapUrdmsUsers(collection.Parties);
            vm.NonUrdmsUsers = MapNonUrdmsUsers(collection.Parties);
            vm.ProjectTitle = project.Title;

            return vm;
        }

        private static void MapForCodes(IList<ClassificationBase> vmForCodes, IList<DataCollectionFieldOfResearch> entityForCodes)
        {
            if (entityForCodes.IsNotEmpty())
            {
                var newResearchList = (from o in entityForCodes
                                      where !vmForCodes.Any(q => o.Code.Id == q.Code.Id)
                                      select o).ToList();

                if (newResearchList.IsNotEmpty())
                {
                    vmForCodes.AddRange(newResearchList);
                }
            }
        }

        private static void MapSeoCodes(IList<ClassificationBase> vmSeoCodes, IList<DataCollectionSocioEconomicObjective> entitySeoCodes)
        {
            if (entitySeoCodes.IsNotEmpty())
            {
                var newResearchList = (from o in entitySeoCodes
                                       where !vmSeoCodes.Any(q => o.Code.Id == q.Code.Id)
                                       select o).ToList();

                if (newResearchList.IsNotEmpty())
                {
                    vmSeoCodes.AddRange(newResearchList);
                }
            }
        }

        private static Party MapManager(IList<DataCollectionParty> entityParties)
        {
            return entityParties.Where(p => p.Party.UserId != null && p.Relationship == DataCollectionRelationshipType.Manager).Single().Party;
        }

        private static IList<UrdmsUserViewModel> MapUrdmsUsers(IList<DataCollectionParty> entityParties)
        {
            return entityParties
               .Where(
                   p =>
                   p.Relationship != DataCollectionRelationshipType.Manager &&
                   !string.IsNullOrWhiteSpace(p.Party.UserId))
               .Select(
                   p =>
                   new UrdmsUserViewModel
                   {
                       UserId = p.Party.UserId,
                       FullName = p.Party.FullName,
                       PartyId = p.Party.Id,
                       Id = p.Id,
                       Relationship = (int)p.Relationship
                   })
               .ToList();
        }

        private static IList<NonUrdmsUserViewModel> MapNonUrdmsUsers(IList<DataCollectionParty> entityParties)
        {
            return entityParties
               .Where(p => string.IsNullOrWhiteSpace(p.Party.UserId))
               .Select(
                   p =>
                   new NonUrdmsUserViewModel
                   {
                       FullName = p.Party.FullName,
                       PartyId = p.Party.Id,
                       Id = p.Id,
                       Relationship = (int)p.Relationship
                   })
               .ToList();
        }
    }
}
