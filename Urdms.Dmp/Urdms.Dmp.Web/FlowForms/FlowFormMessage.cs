using System;
using System.ComponentModel;
using System.IO;
using System.Web;
using Curtin.Framework.Common.Extensions;
using Urdms.Dmp.Web.FlowForms.Helpers;

namespace Urdms.Dmp.Web.FlowForms
{

    public class FlowFormMessage : IDisposable
    {
        private readonly TextWriter _writer;

        public FlowFormMessage(TextWriter writer, MessageType messageType, string heading, IHtmlString message = null)
        {
            _writer = writer;
            _writer.Write(HelperDefinitions.BeginMessage(messageType.ToDescription(), heading).ToHtmlString());

            if (message != null)
            {
                _writer.Write(message.ToHtmlString());
                _writer.Write(HelperDefinitions.EndMessage().ToHtmlString());
            }
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
        /// Outputs the rest of the navigation area.
        /// </summary>
        public void Dispose()
        {
            _writer.Write(HelperDefinitions.EndMessage().ToHtmlString());
        }
    }

    public enum MessageType
    {
        [Description("confirmation")]
        Confirmation,
        [Description("error")]
        Error
    }
}