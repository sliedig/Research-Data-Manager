using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using Curtin.Framework.Common.Classes;
using Curtin.Framework.Common.Extensions;
using Urdms.Dmp.Web.Pages;

namespace Urdms.Dmp.Web.FlowForms.Helpers
{
    public static class Helper
    {
        public static IDictionary<string, object> ObjectToDictionary(object anonymousObject)
        {
            var dictionary = new Dictionary<string, object>();
            if (anonymousObject != null)
            {
                foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(anonymousObject))
                {
                    dictionary.Add(descriptor.Name.Replace('_', '-'), descriptor.GetValue(anonymousObject) ?? "");
                }
            }
            return dictionary;
        }

        public static FlowForm<TModel> BeginFlowForm<TModel>(this HtmlHelper<TModel> helper, string action = null, string controller = null, RouteValueDictionary routeValues = null, FlowFormFormat format = FlowFormFormat.Vertical, bool completed = false, string id = null, FormEncType encType = FormEncType.UrlEncoded, FormMethod method = FormMethod.Post, object htmlAttributes = null)
        {
            var htmlAttrs = ObjectToDictionary(htmlAttributes);

            // Disable unobtrusive JS to stop HTML5 attributes getting printed out
            helper.EnableUnobtrusiveJavaScript(false);

            // Id attribute
            if (id != null)
            {
                htmlAttrs["id"] = id;
            }

            // Class attribute
            if (!htmlAttrs.ContainsKey("class"))
            {
                htmlAttrs["class"] = "";
            }
            var classBuilder = new StringBuilder(htmlAttrs["class"] as string);
            classBuilder.Append(" flow");
            if (format == FlowFormFormat.Horizontal)
            {
                classBuilder.Append(" condensed");
            }
            if (completed)
            {
                classBuilder.Append(" completed");
            }
            if(!helper.ViewData.ModelState.IsValid)
            {
                classBuilder.Append(" erroneous");
            }
            htmlAttrs["class"] = classBuilder.ToString().Trim();

            // Enctype attribute
            htmlAttrs["enctype"] = encType.ToDescription();

            return new FlowForm<TModel>(helper, helper.BeginForm(action, controller, routeValues, method, htmlAttrs));
        }

        public static void FlowFormCssAndJs<T>(BasePage<T> page, FlowFormEnvironment environment = FlowFormEnvironment.Production, string version = "2")
        {
            page.AddScript("/Scripts/forms.js");
            page.AddStylesheet("/Content/Css/forms.css");
        }
    }
}