using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using Urdms.DocumentBuilderService.Database.Entities;
using Urdms.DocumentBuilderService.Models.Enums;

namespace Urdms.DocumentBuilderService.Extensions
{
    public static class DataManagementPlanExtensions
    {
        public static string ToBooleanString(this bool? value, string emptyValue = "", string trueValue = "Yes", string falseValue = "No")
        {
            if (value == null)
            {
                return emptyValue;
            }
            return value == true ? trueValue : falseValue;
        }

        public static XDocument ToXDocument(this DataManagementPlan entity)
        {
            if (entity == null)
            {
                return null;
            }
            var doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement("dataManagementPlan"));
            Debug.Assert(doc.Root != null, "root is null");
            doc.Root.Add(
                entity.GetPreExistingResearchData(),
                entity.GetNewResearchData(),
                entity.GetRelationshipBetweenNewAndPreExistingData(),
                entity.GetBackup(),
                entity.GetDocumentation(),
                entity.GetDataOrganisation(),
                entity.GetEthics(),
                entity.GetPrivacy(),
                entity.GetAccess(),
                entity.GetDataStorage(),
                entity.GetDataRetention(),
                entity.GetDataSharing());

            return doc;


        }

        #region Pre-existing research data
        public static XElement GetPreExistingResearchData(this IPreExistingResearchData entity, string name = "pre-existing-research-data")
        {
            var accessTypes = new XElement("access-types", entity.ExistingDataAccessTypes.GetXmlFlags("access-type", ExistingDataAccessTypes.None));
            accessTypes.Add(new XAttribute("details", entity.AccessTypesDescription ?? string.Empty));
            return new XElement(name,
                new XElement("use-data",entity.UseExistingData.ToBooleanString().ToLower()),
                new XElement("owner", entity.ExistingDataOwner ?? string.Empty),
                accessTypes);
        }
        #endregion

        #region New Research Data
        public static XElement GetNewResearchData(this INewResearchData entity, string name = "new-research-data")
        {
            var owners = new XElement("owners", entity.DataOwners.GetXmlFlags("owner", DataOwners.None));
            owners.Add(new XAttribute("details", entity.DataOwnersDescription ?? string.Empty));

            return new XElement(name,
                new XElement("data-description", entity.ResearchDataDescription ?? string.Empty),
                new XElement("data-action-frequency", entity.DataActionFrequency.GetDescription()),
                new XElement("is-versioned", entity.IsVersioned.ToBooleanString().ToLower()),
                owners);
        }
        #endregion

        #region Relationship
        public static XElement GetRelationshipBetweenNewAndPreExistingData(this IRelationshipBetweenNewAndPreExistingData entity, string name = "relationship-between-pre-existing-and-new-data")
        {
            return new XElement(name, entity.RelationshipBetweenExistingAndNewData.GetDescription());
        }
        #endregion

        #region Backup
        public static XElement GetBackup(this IBackupPolicy entity, string name = "backup-policy")
        {
            var locations = new XElement("locations", entity.BackupLocations.GetXmlFlags("location"));
            var responsibilities = new XElement("responsibilities", entity.BackupPolicyResponsibilities.GetXmlFlags("responsibility", DataResponsibilities.None));
            locations.Add(new XAttribute("details", entity.BackupPolicyLocationsDescription ?? string.Empty));
            responsibilities.Add(new XAttribute("details", entity.BackupPolicyResponsibilitiesDescription ?? string.Empty));

            return new XElement(name,
                                locations,
                                responsibilities);

        } 
        #endregion

        #region Documentation
        public static XElement GetDocumentation(this IDocumentation entity, string name = "documentation")
        {

            return new XElement(name,
                                new XElement("metadata-standards", entity.MetadataStandards ?? string.Empty));
                                
        } 
        #endregion

        #region DataOrganisation
        public static XElement GetDataOrganisation(this IDataOrganisation entity, string name = "data-organisation")
        {

            return new XElement(name,
                                new XElement("directory-structure", entity.DirectoryStructure ?? string.Empty));
        } 
        #endregion
        
        #region DataRetention
        public static XElement GetDataRetention(this IDataRetention entity, string name = "data-retention")
        {
            var responsibilities = new XElement("responsibilities", entity.DataRetentionResponsibilities.GetXmlFlags("responsibility", DataResponsibilities.None));
            responsibilities.Add(new XAttribute("details", entity.DataRetentionResponsibilitiesDescription ?? string.Empty));
            var locations = new XElement("locations", entity.DataRetentionLocations.GetXmlFlags("location", DataRetentionLocations.None));
            locations.Add(new XAttribute("details", entity.DataRetentionLocationsDescription ?? string.Empty));
            return new XElement(name,
                                new XElement("retention-period", entity.DataRetentionPeriod.GetDescription()),
                                new XElement("deposit-to-repository", entity.DepositToRepository.ToBooleanString().ToLower()),
                                responsibilities,
                                locations);
        } 
        #endregion

        #region DataSharing
        public static XElement GetDataSharing(this IDataSharing entity, string name = "data-sharing")
        {
            var availability = new XElement("availability", 
                new XAttribute("date", string.Format("{0:yyyy-MM-dd}", entity.DataSharingAvailabilityDate)),
                entity.DataSharingAvailability.GetDescription());
            var access = new XElement("access",
                                      new XAttribute("details", entity.ShareAccessDescription ?? string.Empty),
                                      entity.ShareAccess.GetDescription());
            return new XElement(name,
                                availability,
                                access,
                                new XElement("licensing", entity.DataLicensingType.GetDescription()),
                                new XElement("interested-organisations", entity.ReuseByOrganisations ?? string.Empty));

        } 
        #endregion

        #region Ethics
        public static XElement GetEthics (this IEthics entity, string name = "ethics")
        {
            return new XElement(name,
                new XElement("requires-clearance",entity.EthicRequiresClearance.ToBooleanString().ToLower()),
                new XElement("details", entity.EthicComments ?? string.Empty));
        }
        #endregion

        #region Privacy
        public static XElement GetPrivacy(this IConfidentiality entity, string name = "privacy")
        {
            return new XElement(name,
                new XElement("is-sensitive",entity.IsSensitive.ToBooleanString().ToLower()),
                new XElement("details", entity.ConfidentialityComments ?? string.Empty));
        }
        #endregion

        #region Data Storage
        public static XElement GetDataStorage(this IDataStorage entity, string name = "data-storage")
        {
            var versionControls = new XElement("version-controls", entity.VersionControl.GetXmlFlags("version-control", VersionControl.None));
            versionControls.Add(new XAttribute("details", entity.VersionControlDescription ?? string.Empty));

            var institutionalStorage = new XElement("institutional-storage", entity.InstitutionalStorageTypes.GetXmlFlags("storage"));
            institutionalStorage.Add(new XAttribute("details", entity.InstitutionalOtherTypeDescription ?? string.Empty));

            var externalStorage = new XElement("external-storage", entity.ExternalStorageTypes.GetXmlFlags("storage", ExternalStorageTypes.None));
            externalStorage.Add(new XAttribute("details", entity.ExternalOtherTypeDescription ?? string.Empty));

            var personalStorage = new XElement("personal-storage", entity.PersonalStorageTypes.GetXmlFlags("storage", PersonalStorageTypes.None));
            personalStorage.Add(new XAttribute("details", entity.PersonalOtherTypeDescription ?? string.Empty));

            return new XElement(name,
                institutionalStorage,
                externalStorage,
                personalStorage,
                new XElement("estimate-size", entity.MaxDataSize.GetDescription()),
                new XElement("file-formats", entity.FileFormats ?? string.Empty),
                versionControls);
        }
        #endregion


        #region Access
        public static  XElement GetAccess(this IAccess entity, string name = "access")
        {
            var urdmsUsers = from o in entity.UrdmsUsers
                              select new XElement("urdms-user", new XAttribute("user-id", o.Id), new XAttribute("full-name", o.Name), new XAttribute("role", ((AccessRole) o.Role).GetDescription()));

            var nonUrdmsUsers = from o in entity.NonUrdmsUsers
                                 select new XElement("non-urdms-user", new XAttribute("full-name", o.Name), new XAttribute("role", ((AccessRole) o.Role).GetDescription()));

            return new XElement(name,
                new XElement("urdms-users", urdmsUsers),
                new XElement("non-urdms-users", nonUrdmsUsers));
            
        }
        #endregion

        #region Helper Extension Methods
        public static IEnumerable<XElement> GetXmlFlags(this Enum flagValue, string name = "item", params Enum[] exclusions)
        {
            var t = flagValue.GetType();
            if (!t.IsDefined(typeof(FlagsAttribute), false))
            {
                throw new InvalidOperationException();
            }

            var values = Enum.GetValues(t).Cast<Enum>().Except(exclusions ?? new Enum[] { });
            var p = from value in values
                    where flagValue.HasFlag(value)
                    select new XElement(name, value.GetDescription());
            return p;
        } 
        #endregion

    }
}
