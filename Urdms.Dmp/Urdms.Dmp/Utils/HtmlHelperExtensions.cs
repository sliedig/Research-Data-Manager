using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Curtin.Framework.Common.Extensions;

namespace Urdms.Dmp.Utils
{
    public static class HtmlHelperExtensions
    {


        public static MvcHtmlString NonPersistentFieldFor<TModel,TProperty>(this HtmlHelper<TModel> model, Expression<Func<TModel,TProperty>> expression, string label = null, string @class = null)
        {
            var fieldName = expression.GetFieldName();
            if (string.IsNullOrWhiteSpace(label))
            {
                var body = expression.Body as MemberExpression;
                if (body != null)
                {
                    var attrib = body.Member.GetCustomAttributes(typeof(DisplayAttribute), false).Cast<DisplayAttribute>().FirstOrDefault();
                    if (attrib != null)
                    {
                        label = attrib.Name;
                    }
                }
            }
            const string format = @"<dt><label for=""{0}"">{1}</label></dt><dd class=""{2}""><input type=""text"" name=""{0}"" id=""{0}"" class=""valid"" /></dd>";
            var html = string.Format(format, fieldName, label, @class);
            return MvcHtmlString.Create(html);
        }

        /// <summary>
        /// Displays the lib guide for item.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="item">The instructions item</param>
        /// <param name="account">The account which defaults to 0</param>
        public static MvcHtmlString LibGuideFor<TModel>(this HtmlHelper<TModel> helper, int item, int account = 0)
        {
            return helper.Action("Index", "LibGuide", new {account, item});
        }

        /// <summary>
        /// Generates the FAQ place holder.
        /// </summary>
        /// <param name="values">The values (a FaqItem list).</param>
        /// <returns>The placeholder markup</returns>
        public static string GenerateFaqPlaceHolder(this IEnumerable<FaqItem> values)
        {
            if (values == null || values.Count() == 0)
            {
                return "";
            }

            const string startFaq = @"<div class=""info""><ul>";
            var result = string.Join("", values.Select(o => string.Format(@"<li><a rel=""prettyPhoto"" href=""{0}"" class=""showfaq"">{1}</a></li>", o.Url, o.Text)));
            const string endFaq = "</ul></div>";

            return startFaq + result + endFaq;
        }

        /// <summary>
        /// Labels the format for.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="helper">The helper.</param>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static MvcHtmlString LabelFormatFor<TModel,TValue>(this HtmlHelper<TModel> helper,Expression<Func<TModel,TValue>> expression, params object[] args)
        {
            var expressionText = helper.LabelFor(expression).ToHtmlString();
            var label = string.Format(expressionText, args);
            return MvcHtmlString.Create(label);
        }
    }

    public class FaqItem
    {
        public string Url { get; set; }
        public string Text { get; set; }
    }
}
