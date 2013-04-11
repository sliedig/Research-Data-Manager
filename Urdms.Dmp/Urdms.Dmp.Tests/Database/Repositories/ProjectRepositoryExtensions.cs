using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Curtin.Framework.Common.Extensions;
using Urdms.Dmp.Database.Entities;

namespace Urdms.Dmp.Tests.Database.Repositories
{
    internal static class ProjectRepositoryExtensions
    {
        private static bool IsEqualsTo(this string text1, string text2)
        {
            return (text1 ?? "") == (text2 ?? "");
        }

        private static bool IsEqualsTo(this DateTime? date1, DateTime? date2)
        {
            return (date1 ?? DateTime.MinValue) == (date2 ?? DateTime.MinValue);
        }

        private static Expression<Func<Project, Project, bool>> GetProjectComparer()
        {
            return (x, y) => (x == null && y == null) ||
                             (x != null && y != null &&
                                 x.Id == y.Id &&
                                 x.Description.IsEqualsTo(y.Description) &&
                                 x.Title.IsEqualsTo(y.Title) &&
                                 x.SourceId == y.SourceId &&
                                 x.SourceProjectType == y.SourceProjectType &&
                                 x.StartDate.IsEqualsTo(y.StartDate) &&
                                 x.EndDate.IsEqualsTo(y.EndDate) &&
                                 x.Status == y.Status &&
                                 x.ProvisioningStatus == y.ProvisioningStatus);
        }

        private static Expression<Func<Project, Project, bool>> GetDataRelationshipDetailComparer()
        {
            return (x, y) => (x == null && y == null) || 
                             (x != null && y != null && (
                                (x.DataManagementPlan == null && y.DataManagementPlan == null) ||
                                (x.DataManagementPlan != null && y.DataManagementPlan != null && (
                                    (x.DataManagementPlan.DataRelationshipDetail == null && y.DataManagementPlan.DataRelationshipDetail == null) ||
                                    (x.DataManagementPlan.DataRelationshipDetail != null && y.DataManagementPlan.DataRelationshipDetail != null &&
                                        x.DataManagementPlan.DataRelationshipDetail.RelationshipBetweenExistingAndNewData == y.DataManagementPlan.DataRelationshipDetail.RelationshipBetweenExistingAndNewData)))));
        }

        private static Expression<Func<Project, Project, bool>> GetNewDataDetailComparer()
        {
            return (x, y) => (x == null && y == null) || 
                             (x != null && y != null && (
                                (x.DataManagementPlan == null && y.DataManagementPlan == null) ||
                                (x.DataManagementPlan != null && y.DataManagementPlan != null && ( 
                                    (x.DataManagementPlan.NewDataDetail == null && y.DataManagementPlan.NewDataDetail == null) ||
                                    (x.DataManagementPlan.NewDataDetail != null && y.DataManagementPlan.NewDataDetail != null &&
                                        x.DataManagementPlan.NewDataDetail.DataActionFrequency == y.DataManagementPlan.NewDataDetail.DataActionFrequency &&
                                        x.DataManagementPlan.NewDataDetail.DataOwners == y.DataManagementPlan.NewDataDetail.DataOwners &&
                                        x.DataManagementPlan.NewDataDetail.DataOwnersDescription.IsEqualsTo(y.DataManagementPlan.NewDataDetail.DataOwnersDescription) &&
                                        x.DataManagementPlan.NewDataDetail.IsVersioned == y.DataManagementPlan.NewDataDetail.IsVersioned &&
                                        x.DataManagementPlan.NewDataDetail.ResearchDataDescription.IsEqualsTo(y.DataManagementPlan.NewDataDetail.ResearchDataDescription))))));

        }

        private static Expression<Func<Project, Project, bool>> GetBackupPolicyComparer()
        {
            return (x, y) => (x == null && y == null) || 
                             (x != null && y != null && (
                                (x.DataManagementPlan == null && y.DataManagementPlan == null) ||
                                (x.DataManagementPlan != null && y.DataManagementPlan != null && ( 
                                    (x.DataManagementPlan.BackupPolicy == null && y.DataManagementPlan.BackupPolicy == null) ||
                                    (x.DataManagementPlan.BackupPolicy != null && y.DataManagementPlan.BackupPolicy != null &&
                                        x.DataManagementPlan.BackupPolicy.BackupLocations == y.DataManagementPlan.BackupPolicy.BackupLocations &&
                                        x.DataManagementPlan.BackupPolicy.BackupPolicyLocationsDescription.IsEqualsTo(y.DataManagementPlan.BackupPolicy.BackupPolicyLocationsDescription) &&
                                        x.DataManagementPlan.BackupPolicy.BackupPolicyResponsibilities == y.DataManagementPlan.BackupPolicy.BackupPolicyResponsibilities &&
                                        x.DataManagementPlan.BackupPolicy.BackupPolicyResponsibilitiesDescription.IsEqualsTo(y.DataManagementPlan.BackupPolicy.BackupPolicyResponsibilitiesDescription))))));

        }

        private static Expression<Func<Project, Project, bool>> GetDataDocumentationComparer()
        {
            return (x, y) => (x == null && y == null) || 
                             (x != null && y != null && (
                                (x.DataManagementPlan == null && y.DataManagementPlan == null) ||
                                (x.DataManagementPlan != null && y.DataManagementPlan != null && (
                                    (x.DataManagementPlan.DataDocumentation == null && y.DataManagementPlan.DataDocumentation == null) ||
                                    (x.DataManagementPlan.DataDocumentation != null && y.DataManagementPlan.DataDocumentation != null &&
                                        x.DataManagementPlan.DataOrganisation.DirectoryStructure.IsEqualsTo(y.DataManagementPlan.DataOrganisation.DirectoryStructure) &&
                                        x.DataManagementPlan.DataDocumentation.MetadataStandards.IsEqualsTo(y.DataManagementPlan.DataDocumentation.MetadataStandards))))));
        }

        private static Expression<Func<Project, Project, bool>> GetDataRetentionComparer()
        {
            return (x, y) => (x == null && y == null) || 
                             (x != null && y != null && (
                                (x.DataManagementPlan == null && y.DataManagementPlan == null) ||
                                (x.DataManagementPlan != null && y.DataManagementPlan != null && (
                                    (x.DataManagementPlan.DataRetention == null && y.DataManagementPlan.DataRetention == null) ||
                                    (x.DataManagementPlan.DataRetention != null && y.DataManagementPlan.DataRetention != null &&
                                        x.DataManagementPlan.DataRetention.DataRetentionLocations == y.DataManagementPlan.DataRetention.DataRetentionLocations &&
                                        x.DataManagementPlan.DataRetention.DataRetentionLocationsDescription.IsEqualsTo(y.DataManagementPlan.DataRetention.DataRetentionLocationsDescription) &&
                                        x.DataManagementPlan.DataRetention.DataRetentionResponsibilities == y.DataManagementPlan.DataRetention.DataRetentionResponsibilities &&
                                        x.DataManagementPlan.DataRetention.DataRetentionResponsibilitiesDescription.IsEqualsTo(y.DataManagementPlan.DataRetention.DataRetentionResponsibilitiesDescription) &&
                                        x.DataManagementPlan.DataRetention.DataRetentionPeriod == y.DataManagementPlan.DataRetention.DataRetentionPeriod)))));
        }

        private static Expression<Func<Project, Project, bool>> GetDataSharingComparer()
        {
            return (x, y) => (x == null && y == null) || 
                             (x != null && y != null && (
                                (x.DataManagementPlan == null && y.DataManagementPlan == null) || 
                                (x.DataManagementPlan != null && y.DataManagementPlan != null && (
                                    (x.DataManagementPlan.DataSharing == null && y.DataManagementPlan.DataSharing == null) ||
                                    (x.DataManagementPlan.DataSharing != null && y.DataManagementPlan.DataSharing != null &&
                                        x.DataManagementPlan.DataSharing.DataSharingAvailability == y.DataManagementPlan.DataSharing.DataSharingAvailability &&
                                        x.DataManagementPlan.DataSharing.DataSharingAvailabilityDate.IsEqualsTo(y.DataManagementPlan.DataSharing.DataSharingAvailabilityDate) &&
                                        x.DataManagementPlan.DataSharing.ReuseByOrganisations.IsEqualsTo(y.DataManagementPlan.DataSharing.ReuseByOrganisations) &&
                                        x.DataManagementPlan.DataSharing.ShareAccess == y.DataManagementPlan.DataSharing.ShareAccess)))));
        }

        private static Expression<Func<Project, Project, bool>> GetIntellectualPropertyComparer()
        {
            return (x, y) => (x == null && y == null) || 
                             (x != null && y != null && (
                                (x.DataManagementPlan == null && y.DataManagementPlan == null) ||
                                (x.DataManagementPlan != null && y.DataManagementPlan != null && (
                                    (x.DataManagementPlan.Confidentiality == null && y.DataManagementPlan.Confidentiality == null) ||
                                    (x.DataManagementPlan.Confidentiality != null && y.DataManagementPlan.Confidentiality != null &&
                                        x.DataManagementPlan.Confidentiality.ConfidentialityComments.IsEqualsTo(y.DataManagementPlan.Confidentiality.ConfidentialityComments) &&
                                        x.DataManagementPlan.Confidentiality.IsSensitive == y.DataManagementPlan.Confidentiality.IsSensitive)))));
        }

        private static Expression<Func<Project, Project, bool>> GetDataStorageComparer()
        {
            return (x, y) => (x == null && y == null) ||
                             (x != null && y != null && (
                                (x.DataManagementPlan == null && y.DataManagementPlan == null) ||
                                (x.DataManagementPlan != null && y.DataManagementPlan != null && (
                                    (x.DataManagementPlan.DataStorage == null && y.DataManagementPlan.DataStorage == null) ||
                                    (x.DataManagementPlan.DataStorage != null && y.DataManagementPlan.DataStorage != null &&
                                        x.DataManagementPlan.DataStorage.InstitutionalStorageTypes == y.DataManagementPlan.DataStorage.InstitutionalStorageTypes &&
                                        x.DataManagementPlan.DataStorage.InstitutionalOtherTypeDescription.IsEqualsTo(y.DataManagementPlan.DataStorage.InstitutionalOtherTypeDescription) &&
                                        x.DataManagementPlan.DataStorage.ExternalStorageTypes == y.DataManagementPlan.DataStorage.ExternalStorageTypes &&
                                        x.DataManagementPlan.DataStorage.ExternalOtherTypeDescription.IsEqualsTo(y.DataManagementPlan.DataStorage.ExternalOtherTypeDescription) &&
                                        x.DataManagementPlan.DataStorage.PersonalStorageTypes == y.DataManagementPlan.DataStorage.PersonalStorageTypes &&
                                        x.DataManagementPlan.DataStorage.PersonalOtherTypeDescription.IsEqualsTo(y.DataManagementPlan.DataStorage.PersonalOtherTypeDescription) &&
                                        x.DataManagementPlan.DataStorage.MaxDataSize == y.DataManagementPlan.DataStorage.MaxDataSize &&
                                        x.DataManagementPlan.DataStorage.FileFormats.IsEqualsTo(y.DataManagementPlan.DataStorage.FileFormats) &&
                                        x.DataManagementPlan.ExistingDataDetail.ExistingDataOwner.IsEqualsTo(y.DataManagementPlan.ExistingDataDetail.ExistingDataOwner) &&
                                        x.DataManagementPlan.ExistingDataDetail.UseExistingData == y.DataManagementPlan.ExistingDataDetail.UseExistingData)))));
        }

        private static Expression<Func<Project, Project, bool>> GetExistingDataDetailComparer()
        {
            return (x, y) => (x == null && y == null) || 
                             (x != null && y != null && (
                                (x.DataManagementPlan == null && y.DataManagementPlan == null) || 
                                (x.DataManagementPlan != null && y.DataManagementPlan != null && (
                                    (x.DataManagementPlan.ExistingDataDetail == null && y.DataManagementPlan.ExistingDataDetail == null) ||
                                    (x.DataManagementPlan.ExistingDataDetail != null && y.DataManagementPlan.ExistingDataDetail != null &&
                                        x.DataManagementPlan.ExistingDataDetail.AccessTypesDescription.IsEqualsTo(y.DataManagementPlan.ExistingDataDetail.AccessTypesDescription) &&
                                        x.DataManagementPlan.ExistingDataDetail.ExistingDataAccessTypes == y.DataManagementPlan.ExistingDataDetail.ExistingDataAccessTypes &&
                                        x.DataManagementPlan.ExistingDataDetail.ExistingDataOwner.IsEqualsTo(y.DataManagementPlan.ExistingDataDetail.ExistingDataOwner) &&
                                        x.DataManagementPlan.ExistingDataDetail.UseExistingData == y.DataManagementPlan.ExistingDataDetail.UseExistingData)))));
        }

        private static Expression<Func<Project, Project, bool>> GetMembersComparer()
        {
            return (x, y) => (x == null && y == null) ||
                             (x != null && y != null && (
                                (x.Parties.IsEmpty() && y.Parties.IsEmpty()) ||
                                (x.Parties.IsNotEmpty() && y.Parties.IsNotEmpty() &&
                                    x.Parties.IntersectByComparer(y.Parties, GetMemberComparer()).Count() == x.Parties.Count)));
        }


        private static Func<ProjectParty, ProjectParty, bool> GetMemberComparer()
        {
            return (x, y) => (x == null && y == null) ||
                             (x != null && y != null &&
                              x.Party.Id == y.Party.Id &&
                              x.Party.UserId.IsEqualsTo(y.Party.UserId) &&
                              x.Party.Organisation.IsEqualsTo(y.Party.Organisation) &&
                              x.Party.FullName.IsEqualsTo(y.Party.FullName) &&
                              x.Role == y.Role);
        }

        private static Expression<Func<Project, Project, bool>> GetSocioEconomicResearchListComparer()
        {
            return (x, y) => (x == null && y == null) ||
                            (x != null && y != null && (
                               (x.SocioEconomicObjectives.IsEmpty() && y.SocioEconomicObjectives.IsEmpty()) ||
                               (x.SocioEconomicObjectives.IsNotEmpty() && y.SocioEconomicObjectives.IsNotEmpty() &&
                                   x.SocioEconomicObjectives.IntersectByComparer(y.SocioEconomicObjectives, GetSocioEconomicResearchComparer()).Count() == x.FieldsOfResearch.Count)));
        }

        private static Expression<Func<Project, Project, bool>> GetFieldOfResearchListComparer()
        {
            return (x, y) => (x == null && y == null) ||
                            (x != null && y != null && (
                               (x.FieldsOfResearch.IsEmpty() && y.FieldsOfResearch.IsEmpty()) ||
                               (x.FieldsOfResearch.IsNotEmpty() && y.FieldsOfResearch.IsNotEmpty() &&
                                   x.FieldsOfResearch.IntersectByComparer(y.FieldsOfResearch, GetFieldOfResearchComparer()).Count() == x.FieldsOfResearch.Count)));
        }

        private static Func<ProjectSocioEconomicObjective,ProjectSocioEconomicObjective,bool> GetSocioEconomicResearchComparer()
        {
            return (x, y) => (x == null && y == null) ||
                             (x != null && y != null && (
                                (x.SocioEconomicObjective == null && y.SocioEconomicObjective == null) ||
                                (x.SocioEconomicObjective != null && y.SocioEconomicObjective != null &&
                                x.SocioEconomicObjective.Id == y.SocioEconomicObjective.Id)));
        }

        private static Func<ProjectFieldOfResearch, ProjectFieldOfResearch, bool> GetFieldOfResearchComparer()
        {
            return (x, y) => (x == null && y == null) ||
                             (x != null && y != null && (
                                (x.FieldOfResearch == null && y.FieldOfResearch == null) ||
                                (x.FieldOfResearch != null && y.FieldOfResearch != null &&
                                x.FieldOfResearch.Id == y.FieldOfResearch.Id)));
        }

        private static Expression<Func<Project, Project, bool>> GetDataManagementPlanComparer()
        {
            return (x, y) => (x == null && y == null) ||
                             (x != null && y != null && (
                                (x.DataManagementPlan == null && y.DataManagementPlan == null) ||
                                (x.DataManagementPlan != null && y.DataManagementPlan != null &&
                                    x.DataManagementPlan.Id == y.DataManagementPlan.Id)));
              
        }


        private static Expression<Func<Project, Project, bool>> GetEthicComparer()
        {
            return (x, y) => (x == null && y == null) ||
                             (x != null && y != null && (
                                (x.DataManagementPlan == null && y.DataManagementPlan == null) ||
                                (x.DataManagementPlan != null && y.DataManagementPlan != null && (
                                    (x.DataManagementPlan.Ethic == null && y.DataManagementPlan.Ethic == null) ||
                                    (x.DataManagementPlan.Ethic != null && y.DataManagementPlan.Ethic != null &&
                                        x.DataManagementPlan.Ethic.EthicComments.IsEqualsTo(y.DataManagementPlan.Ethic.EthicComments) &&
                                        x.DataManagementPlan.Ethic.EthicRequiresClearance == y.DataManagementPlan.Ethic.EthicRequiresClearance)))));
        }


        public static bool IsEqualTo(this Project source, Project other, ProjectCompareTypes comparison)
        {
            var expressions = GetExpressions(comparison).ToList();
            if (expressions.IsEmpty())
            {
                return false;
            }
            var expression = expressions.First();
            foreach (var item in expressions.Skip(1))
            {
                expression = expression.AndAlso(item);
            }
            var lambda = expression.Compile();
            return lambda(source, other);

        }

        private static IEnumerable<Expression<Func<Project, Project, bool>>> GetExpressions(ProjectCompareTypes comparison)
        {
            if (comparison.HasCompareTypes(ProjectCompareTypes.Basic))
            {
                yield return GetProjectComparer();
                yield return GetMembersComparer();
                yield return GetSocioEconomicResearchListComparer();
                yield return GetFieldOfResearchListComparer();
            }
            else
            {
                if (comparison.HasCompareTypes(ProjectCompareTypes.Project))
                {
                    yield return GetProjectComparer();
                }
                if (comparison.HasCompareTypes(ProjectCompareTypes.DataManagementPlan))
                {
                    yield return GetDataManagementPlanComparer();
                }
                if (comparison.HasCompareTypes(ProjectCompareTypes.Members,ProjectCompareTypes.Project))
                {
                    yield return GetMembersComparer();
                }
                if (comparison.HasCompareTypes(ProjectCompareTypes.SocioEconomicResearch, ProjectCompareTypes.Project))
                {
                    yield return GetSocioEconomicResearchListComparer();
                }
                if (comparison.HasCompareTypes(ProjectCompareTypes.FieldOfResearch, ProjectCompareTypes.Project))
                {
                    yield return GetFieldOfResearchListComparer();
                }
                if (comparison.HasCompareTypes(ProjectCompareTypes.DataStorage))
                {
                    yield return GetDataStorageComparer();
                }
                if (comparison.HasCompareTypes(ProjectCompareTypes.BackupPolicy))
                {
                    yield return GetBackupPolicyComparer();
                }
                if (comparison.HasCompareTypes(ProjectCompareTypes.DataDocumentation))
                {
                    yield return GetDataDocumentationComparer();
                }
                if (comparison.HasCompareTypes(ProjectCompareTypes.DataRetention))
                {
                    yield return GetDataRetentionComparer();
                }
                if (comparison.HasCompareTypes(ProjectCompareTypes.DataSharing))
                {
                    yield return GetDataSharingComparer();
                }
                if (comparison.HasCompareTypes(ProjectCompareTypes.ExistingDataDetail))
                {
                    yield return GetExistingDataDetailComparer();
                }
                if (comparison.HasCompareTypes(ProjectCompareTypes.IntellectualProperty))
                {
                    yield return GetIntellectualPropertyComparer();
                }
                if (comparison.HasCompareTypes(ProjectCompareTypes.NewDataDetail))
                {
                    yield return GetNewDataDetailComparer();
                }
                if (comparison.HasCompareTypes(ProjectCompareTypes.Ethic))
                {
                    yield return GetEthicComparer();
                }
                if (comparison.HasCompareTypes(ProjectCompareTypes.DataRelationshipDetail))
                {
                    yield return GetDataRelationshipDetailComparer();
                }

            }
        }

        private static bool HasCompareTypes(this ProjectCompareTypes item, params ProjectCompareTypes[] values)
        {
           if (values.IsEmpty())
           {
               return false;
           }
            return values.Any(o => item.HasFlag(o));
        }
    }
}
