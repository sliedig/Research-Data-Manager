using System;
using System.IO;
using System.Web;
using Urdms.Dmp.Web.FlowForms.Helpers;

namespace Urdms.Dmp.Web.FlowForms
{
    public class FlowFormNavigation : IDisposable
    {
        private readonly TextWriter _writer;

        public FlowFormNavigation(TextWriter writer)
        {
            _writer = writer;
            _writer.Write(HelperDefinitions.BeginNavigation().ToHtmlString());
        }

        /// <summary>
        /// Returns an IHtmlString object that will output a HTML submit button.
        /// </summary>
        /// <param name="value">The string message to show on the button</param>
        /// <param name="id">The id of the button</param>
        /// <param name="name">The name of the button</param>
        /// <param name="classes">Any classes to add to the button</param>
        /// <returns>IHtmlString object that will output a HTML submit button</returns>
        public IHtmlString Submit(string value, string id = null, string classes = null, string name = null)
        {
            return HelperDefinitions.Submit(value, id, classes, name);
        }

        /// <summary>
        /// Returns an IHtmlString object that will output a HTML reset button.
        /// </summary>
        /// <param name="value">The string message to show on the button</param>
        /// <param name="id">The id of the button</param>
        /// <param name="name">The name of the button</param>
        /// <param name="classes">Any classes to add to the button</param>
        /// <returns>IHtmlString object that will output a HTML reset button</returns>
        public IHtmlString Reset(string value, string id = null, string classes = null, string name = null)
        {
            return HelperDefinitions.Submit(value, id, classes, name, true);
        }

        /// <summary>
        /// Outputs the rest of the navigation area.
        /// </summary>
        public void Dispose()
        {
            _writer.Write(HelperDefinitions.EndNavigation().ToHtmlString());
        }
    }
}