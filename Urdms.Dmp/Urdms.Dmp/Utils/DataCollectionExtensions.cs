using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Urdms.Dmp.Database.Entities;

namespace Urdms.Dmp.Utils
{
    public static class DataCollectionExtensions
    {
        public static DataCollectionHashCode NewDataCollectionHashCode(this DataCollection collection)
        {
            var hashCode = new DataCollectionHashCode
            {
                DataCollectionId = collection.Id,
                HashCode = GetDataCollectionHash(collection)
            };


            return hashCode;
        }

        public static string GetDataCollectionHash(this DataCollection collection)
        {
            var text = string.Join("|", GetHashCodeParts(collection));
            var tmpSource = Encoding.UTF8.GetBytes(text);
            var tmpHash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);
            var result = ByteArrayToString(tmpHash);
            return result;
        }

        private static IEnumerable<string> GetHashCodeParts(DataCollection collection)
        {
            yield return string.Format("Id:{0}", collection.Id);
            yield return string.Format("Title:{0}", collection.Title);
            yield return string.Format("ResearchDataDescription:{0}", collection.ResearchDataDescription);
            yield return string.Format("Type:{0}", (int)collection.Type);
            yield return string.Format("StartDate:{0}", collection.StartDate);
            yield return string.Format("EndDate:{0}", collection.EndDate);
            yield return string.Format("DataLicensingRights:{0}", (int)collection.DataLicensingRights);
            yield return string.Format("ShareAccess ShareAccess:{0}", (int)collection.ShareAccess);
            yield return string.Format("ShareAccessDescription:{0}", collection.ShareAccessDescription);
            yield return string.Format("Keywords:{0}", collection.Keywords);
            yield return string.Format("AwareOfEthics:{0}", collection.AwareOfEthics);
            yield return string.Format("Availability:{0}", (int)collection.Availability);
            yield return string.Format("AvailabilityDate:{0}", collection.AvailabilityDate);
            yield return string.Format("EthicsApprovalNumber:{0}", collection.EthicsApprovalNumber);
            yield return string.Format("DataStoreLocationName:{0}", collection.DataStoreLocationName);
            yield return string.Format("DataStoreLocationUrl:{0}", collection.DataStoreLocationUrl);
            yield return string.Format("DataStoreAdditionalDetails:{0}", collection.DataStoreAdditionalDetails);
            yield return string.Format("DataCollectionIdentifier:{0}", (int)collection.DataCollectionIdentifier);
            yield return string.Format("DataCollectionIdentifierValue:{0}", collection.DataCollectionIdentifierValue);
            foreach (var objectives in collection.SocioEconomicObjectives)
            {
                var code = objectives.Code;
                yield return string.Format("SeoCode:{0}", code.Id);
            }

            foreach (var fieldOfResearch in collection.FieldsOfResearch)
            {
                var code = fieldOfResearch.Code;
                yield return string.Format("ForCode:{0}", code.Id);
            }

            foreach (var dataCollectionParty in collection.Parties)
            {
                var party = dataCollectionParty.Party;
                yield return string.Format("Party[Id:{0}-Relationship:{1}-PartyId:{2}-UserId:{3}-FirstName:{4}-LastName:{5}-Email:{6}-Organisation:{7}-FullName:{8}]",
                                           dataCollectionParty.Id,
                                           (int)dataCollectionParty.Relationship,
                                           party.Id,
                                           party.UserId,
                                           party.FirstName,
                                           party.LastName,
                                           party.Email,
                                           party.Organisation,
                                           party.FullName);
            }
        }

        private static string ByteArrayToString(byte[] items)
        {
            var sb = new StringBuilder(items.Length);
            foreach (var item in items)
            {
                sb.Append(item.ToString("X2"));
            }
            return sb.ToString();
        }
    }
}