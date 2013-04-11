using System;
using System.IO;
using System.Web;
using Urdms.Dmp.Web.FlowForms.Helpers;

namespace Urdms.Dmp.Web.FlowForms
{
    public class FlowFormField : IDisposable
    {
        private readonly bool _containsSection;
        private readonly TextWriter _writer;

        public FlowFormField(TextWriter writer, bool containsSection, string labelHtml, string elementHtml, string errorHtml = "", bool isValid = true, IHtmlString hint = null, string tip = null, bool hideTip = true, string hintClass = null, string parentClass = null, bool displayFieldName = true)
        {
            _containsSection = containsSection;
            _writer = writer;

            var generatedSection = HelperDefinitions.BeginField(containsSection, displayFieldName ? new HtmlString(labelHtml) : null, new HtmlString(elementHtml), new HtmlString(errorHtml), isValid, hint, tip, hideTip, hintClass, parentClass);
            _writer.Write(generatedSection.ToHtmlString());
        }

        /// <summary>
        /// Blank ToString() so an object of this class can be "outputted" in the Razor template.
        /// </summary>
        /// <returns>""</returns>
        public override string ToString()
        {
            return "";
        }

        /// <summary>
        /// Finishes off writing the HTML for the field to the view context writer
        /// </summary>
        public void Dispose()
        {
            if (_containsSection)
            {
                _writer.Write(HelperDefinitions.EndField().ToHtmlString());
            }
        }
    }
}