using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;

namespace Urdms.ProvisioningService.SharePoint.Helpers
{
    /// <summary>
    /// Thanks to Steve Curran for providing us with this code
    /// http://sharepointfieldnotes.blogspot.com.au/2011/08/sharepoint-2010-code-tips-setting.html
    /// </summary>
    public static class SharePointTaxonomyHelper
    {
        public static string GetTermInfo(string siteUrl, Field field, string term, ref string hiddentextField)
        {
            string sspId = string.Empty;
            string termSetId = string.Empty;

            XElement schemaRoot = XElement.Parse(field.SchemaXml);

            foreach (XElement property in schemaRoot.Descendants("Property"))
            {
                XElement name = property.Element("Name");
                XElement value = property.Element("Value");

                if (name != null && value != null)
                {
                    switch (name.Value)
                    {
                        case "SspId":
                            sspId = value.Value;
                            break;
                        case "TermSetId":
                            termSetId = value.Value;
                            break;
                        case "TextField":
                            hiddentextField = value.Value;
                            break;
                    }
                }
            }

            string termSetXml = GetTerms(siteUrl, sspId, termSetId);

            XElement termSetElement = XElement.Parse(termSetXml);
            var termId = from t in termSetElement.Descendants("T")
                         where t.Descendants("TL").Attributes("a32").First().Value.ToUpper() == term.ToUpper()
                         select t.Attribute("a9");

            if (termId != null && termId.Count() > 0)
            {
                return termId.First().Value.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        private static string GetTerms(string siteUrl, string sspId, string termSetId)
        {
			//var ts = new termsservice.Taxonomywebservice();
			//ts.UseDefaultCredentials = true;
			//ts.Url = siteUrl + "/_vti_bin/taxonomyclientservice.asmx";

			//string timeStamp;
			//string termSetXml = ts.GetTermSets(WrapXml(sspId), WrapXml(termSetId),
			//                                   CultureInfo.CurrentUICulture.LCID,
			//                                   WrapXml(DateTime.Now.ToUniversalTime().Ticks.ToString()), WrapXml("0"),
			//                                   out timeStamp);

			//return termSetXml;
        	return string.Empty;
        }

        private static string WrapXml(string value)
        {
            return string.Format("<is><i>{0}</i></is>", value);
        }

        #region -- Sample 
     
        /// <summary>
        /// This is a sample of how to invoke helper methods. Also illustrates how to add multiple terms. 
        /// Do Not Use!
        /// </summary>
        private static void SetManagedMetaDataField_ClientOM(string siteUrl, string listName, string itemID, string fieldName, string term)
        {
            ClientContext clientContext = new ClientContext(siteUrl);
            var list = clientContext.Web.Lists.GetByTitle(listName);
            FieldCollection fields = list.Fields;
            Field field = fields.GetByInternalNameOrTitle(fieldName);
            CamlQuery camlQueryForItem = new CamlQuery();
            string queryXml = @"<View><Query><Where><Eq><FieldRef Name='ID'/><Value Type='Counter'>!@itemid</Value></Eq></Where></Query></View>";
            camlQueryForItem.ViewXml = queryXml.Replace("!@itemid", itemID);
            ListItemCollection listItems = list.GetItems(camlQueryForItem);
            clientContext.Load(listItems);
            clientContext.Load(fields);
            clientContext.Load(field);
            clientContext.ExecuteQuery();
            string hiddentTextFieldID = string.Empty;
            string termId = GetTermInfo(siteUrl, field, term, ref hiddentTextFieldID);
            if (!string.IsNullOrEmpty(termId))
            {
                ListItem item = listItems[0];
                string termValue = string.Empty;
                string termHTVal = string.Empty;
                object itemValue = null;
                List<object> objectVals = null;
                if (item[fieldName] != null && item[fieldName].GetType().IsAssignableFrom(typeof(object[])))
                {
                    termValue = term + "|" + termId;
                    objectVals = ((object[])item[fieldName]).ToList<object>();
                    objectVals.Add(termValue);
                    itemValue = objectVals.ToArray<object>();
                    foreach (object val in objectVals)
                    {
                        termHTVal += "-1;#" + val + ";";
                    }
                    termHTVal = termHTVal.Substring(0, termHTVal.Length - 1);
                }
                else
                {
                    termValue = "-1" + ";#" + term + "|" + termId;
                    termHTVal = termValue;
                    itemValue = termValue;
                }
                Field hiddenTextField = fields.GetById(new Guid(hiddentTextFieldID));
                clientContext.Load(hiddenTextField);
                clientContext.ExecuteQuery();
                item[hiddenTextField.InternalName] = termHTVal;
                item[fieldName] = itemValue;
                item.Update();
                clientContext.Load(item);
                clientContext.ExecuteQuery();
            }
        }   
        
        #endregion
    }
}