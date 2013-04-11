using System.Linq;
using Omu.ValueInjecter;
using Urdms.Dmp.Database.Entities;
using Urdms.Dmp.Models;
using Urdms.Dmp.Models.Enums;

namespace Urdms.Dmp.Mappers
{
    public static class DataManagementPlanMapper
    {
        public static DataManagementPlanViewModel MapFrom(this DataManagementPlanViewModel vm, Project entity)
        {
            if (vm == null || entity == null)
                return vm;

            vm.ProjectId = entity.Id;
            vm.ProjectDescription = entity.Description;
            vm.ProjectTitle = entity.Title;
            vm.PrincipalInvestigator = entity.Parties.Where(p => p.Party.UserId != null && p.Relationship == ProjectRelationship.PrincipalInvestigator).Single().Party;
            vm.Status = entity.ProvisioningStatus;
            vm.InjectFrom<SameNameWithRecursion>(entity.DataManagementPlan);
            vm.UrdmsUsers = entity.Parties
                .Where(
                    p =>
                    p.Relationship != ProjectRelationship.PrincipalInvestigator &&
                    !string.IsNullOrWhiteSpace(p.Party.UserId))
                .Select(
                    p =>
                    new UrdmsUserViewModel
                    {
                        UserId = p.Party.UserId,
                        FullName = p.Party.FullName,
                        PartyId = p.Party.Id,
                        Id = p.Id,
                        Relationship = (int)p.Role
                    })
                .ToList();

            vm.NonUrdmsUsers = entity.Parties
                .Where(p => string.IsNullOrWhiteSpace(p.Party.UserId))
                .Select(
                    p =>
                    new NonUrdmsUserViewModel
                    {
                        FullName = p.Party.FullName,
                        PartyId = p.Party.Id,
                        Id = p.Party.Id,
                        Relationship = (int)p.Role
                    })
                .ToList();

            return vm;
        }
    }
}