using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Urdms.Dmp.Database.Entities;
using Urdms.Dmp.Database.Repositories;
using Urdms.Dmp.Models.Interfaces;
using Urdms.Dmp.Utils;

namespace Urdms.Dmp.Controllers.Helpers
{
    public static class ResearchCodesHelper
    {
        public static void PopulateForCodes<T>(this IFieldOfResearchRepository fieldOfResearchRepository, IResearchCodesViewModel vm, NameValueCollection form) where T : ClassificationBase, new()
        {
            var fieldOfResearch = fieldOfResearchRepository.GetFieldOfResearch(vm.FieldOfResearchCode);
            if (fieldOfResearch == null)
            {
                throw new Exception("Field of research code not found, please ensure code is correct")
                          {Source = "FieldOfResearchCode"};
            }

            if (vm.FieldsOfResearch == null)
            {
                vm.FieldsOfResearch = new List<ClassificationBase> { new T { Code = fieldOfResearch } };
            }
            else if (!vm.FieldsOfResearch.Any(o => o.Code.Id == fieldOfResearch.Id))
            {
                vm.FieldsOfResearch.Add(new T {Code = fieldOfResearch});
                vm.FieldsOfResearch = vm.FieldsOfResearch.OrderBy(o => o.Code.Id).ToList();
            }
        }


        public static void PopulateSeoCodes<T>(this ISocioEconomicObjectiveRepository socioEconomicObjectiveRepository, IResearchCodesViewModel vm, NameValueCollection form) where T : ClassificationBase, new()
        {
            var socioEconomicObjective = socioEconomicObjectiveRepository.GetSocioEconomicObjective(vm.SocioEconomicObjectiveCode);
            
            if (socioEconomicObjective == null)
            {
                throw new Exception("Socio economic objective code not found, please ensure code is correct")
                          {Source = "SocioEconomicObjectiveCode"};
            }
            
            if (vm.SocioEconomicObjectives == null)
            {
                vm.SocioEconomicObjectives = new List<ClassificationBase> { new T { Code = socioEconomicObjective } };
            }
            else if (!vm.SocioEconomicObjectives.Any(o => o.Code.Id == socioEconomicObjective.Id))
            {
                vm.SocioEconomicObjectives.Add(new T {Code = socioEconomicObjective});
                vm.SocioEconomicObjectives = vm.SocioEconomicObjectives.OrderBy(o => o.Code.Id).ToList();
            }

        }

        public static void DeleteForCodes(this IResearchCodesViewModel vm, NameValueCollection collection) 
        {
            if (vm.FieldsOfResearch == null || vm.FieldsOfResearch.Count == 0)
            {
                return;
            }
            const string prefix = "RemoveForCode_";
            var deletionKeys = collection.AllKeys
                .Where(o => o.StartsWith(prefix) && collection[o].Contains("true"))
                .Select(o => o.Substring(prefix.Length));

            foreach (var deletedEntity in deletionKeys.Select(deletionKey => vm.FieldsOfResearch.FirstOrDefault(o => o.Code.Id == deletionKey)).Where(deletedEntity => deletedEntity != null))
            {
                vm.FieldsOfResearch.Remove(deletedEntity);
            }
        }

        public static void DeleteSeoCodes(this IResearchCodesViewModel vm, NameValueCollection collection)
        {
            if (vm.SocioEconomicObjectives == null || vm.SocioEconomicObjectives.Count == 0)
            {
                return;
            }
            const string prefix = "RemoveSeoCode_";
            var deletionKeys = collection.AllKeys
                .Where(o => o.StartsWith(prefix) && collection[o].Contains("true"))
                .Select(o => o.Substring(prefix.Length));

            foreach (var deletedEntity in deletionKeys.Select(deletionKey => vm.SocioEconomicObjectives.FirstOrDefault(o => o.Code.Id == deletionKey)).Where(deletedEntity => deletedEntity != null))
            {
                vm.SocioEconomicObjectives.Remove(deletedEntity);
            }
        }


        public static void ExtractForCodes<T>(this IResearchCodesViewModel vm, NameValueCollection collection) where T : ClassificationBase, new()
        {
            if (vm.FieldsOfResearch == null)
            {
                vm.FieldsOfResearch = new List<ClassificationBase>();
            }
            var rows = collection.GetValues("ForCodeRows");
            if (rows != null && rows.Length != 0)
            {
                var forCodes = from o in rows
                                let fields = o.Trim().Split(':')
                                let item = fields.Length == 3 ? new { EntityId = int.Parse(fields[0]), Id = fields[1], Name = fields[2] } : null
                                where item != null && !vm.FieldsOfResearch.Any(q => q.Code.Id == item.Id)
                                select new T
                                           {
                                               Id = item.EntityId,
                                               Code = new FieldOfResearch { Id = item.Id, Name = item.Name },
                                           };
                vm.FieldsOfResearch.AddRange(forCodes);
            }
            vm.FieldsOfResearch = vm.FieldsOfResearch.OrderBy(o => o.Code.Id).ToList();
        }


        public static void ExtractSeoCodes<T>(this IResearchCodesViewModel vm, NameValueCollection collection) where T : ClassificationBase, new()
        {
            if (vm.SocioEconomicObjectives == null)
            {
                vm.SocioEconomicObjectives = new List<ClassificationBase>();
            }
            var rows = collection.GetValues("SeoCodeRows");
            if (rows != null && rows.Length != 0)
            {
                var objectives = from o in rows
                                  let fields = o.Trim().Split(':')
                                  let item = fields.Length == 3 ? new { EntityId = int.Parse(fields[0]), Id = fields[1], Name = fields[2] } : null
                                  where item != null && !vm.SocioEconomicObjectives.Any(q => q.Code.Id == item.Id)
                                  select new T
                                             {
                                                 Id = item.EntityId,
                                                 Code = new SocioEconomicObjective { Id = item.Id, Name = item.Name },
                                             };

                vm.SocioEconomicObjectives.AddRange(objectives);
            }

            vm.SocioEconomicObjectives = vm.SocioEconomicObjectives.OrderBy(o => o.Code.Id).ToList();
        }
    }
}