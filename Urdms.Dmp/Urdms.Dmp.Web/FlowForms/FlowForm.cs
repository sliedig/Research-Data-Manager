using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Urdms.Dmp.Web.FlowForms.Helpers;

namespace Urdms.Dmp.Web.FlowForms
{
    public class FlowForm<TModel> : IDisposable
    {
        private readonly HtmlHelper<TModel> _htmlHelper;
        private readonly MvcForm _form;

        public FlowForm(HtmlHelper<TModel> htmlHelper, MvcForm form)
        {
            _htmlHelper = htmlHelper;
            _form = form;
        }

        /// <summary>
        /// Finish writing the HTML for the form.
        /// </summary>
        public void Dispose()
        {
            _form.Dispose();
        }

        /// <summary>
        /// Render the start of a flow form section.
        /// </summary>
        /// <param name="heading">The heading for the section</param>
        /// <param name="id">The id to give the &lt;fieldset&gt;</param>
        /// <param name="instructions">Any instructions that should be shown at the start of the section</param>
        /// <param name="omitDl">Whether or not to omit the &lt;dl&gt; - only use in special cases when you aren't adding flow form fields to the &lt;fieldset&gt;</param>
        /// <returns>A FlowFormSection object that can be used to create nested fields</returns>
        public FlowFormSection<TModel> BeginSection(string heading, string id = null, IHtmlString instructions = null, bool omitDl = false)
        {
            return new FlowFormSection<TModel>(_htmlHelper, false, heading, id, null, null, false, instructions, omitDl);
        }

        /// <summary>
        /// Render a flow form header.
        /// </summary>
        /// <param name="heading">The heading of the form</param>
        /// <param name="instructions">Any instructions that should be shown at the top of the form</param>
        /// <param name="logo">A URL to an image to use as a logo for the form</param>
        /// <param name="logoAlt">The alt text for the logo; required if logo is specified</param>
        /// <param name="legislation">Legislative text to show with the form</param>
        /// <param name="copyright">A copyright statement for the form; usually in the form of something like: @Html.CheckBoxFor(m => m.Copyright) @Html.LabelFor(m => m.Copyright, "You will accept!")</param>
        /// <returns>An IHtmlString object that will render the header</returns>
        public IHtmlString Header(string heading, IHtmlString instructions = null, string logo = null, string logoAlt = null, IHtmlString legislation = null, IHtmlString copyright = null)
        {
            if (!string.IsNullOrEmpty(logo) && string.IsNullOrEmpty(logoAlt))
            {
                throw new ApplicationException("Logo specified for flow form header, but no alt text; please specify alt text.");
            }
            return HelperDefinitions.Header(heading, instructions, logo, logoAlt, legislation, copyright);
        }

        /// <summary>
        /// Render flow form steps.
        /// </summary>
        /// <param name="numSteps">The number of steps in the form</param>
        /// <param name="current">The current step the user is on</param>
        /// <param name="showLinks">Whether or not to show links against the steps; links are generated in format ?step={stepNo}</param>
        /// <param name="stopLinksAtMaxComplete">Whether or not to show only links to completed steps</param>
        /// <returns>An IHtmlString object that will render the steps</returns>
        public IHtmlString Steps(int numSteps, int current, bool showLinks = false, bool stopLinksAtMaxComplete = false)
        {
            return Steps(numSteps, current, current, showLinks, stopLinksAtMaxComplete);
        }

        /// <summary>
        /// Render flow form steps.
        /// </summary>
        /// <param name="numSteps">The number of steps in the form</param>
        /// <param name="current">The current step the user is on</param>
        /// <param name="maxComplete">The maximum step the user has completed thus far</param>
        /// <param name="showLinks">Whether or not to show links against the steps; links are generated in format ?step={stepNo}</param>
        /// <param name="stopLinksAtMaxComplete">Whether or not to show only links to completed steps</param>
        /// <returns>An IHtmlString object that will render the steps</returns>
        public IHtmlString Steps(int numSteps, int current, int maxComplete, bool showLinks = false, bool stopLinksAtMaxComplete = false)
        {
            var steps = new List<Step>();
            for (int i = 1; i <= numSteps; i++)
            {
                steps.Add(new Step { Name = "Step "+i, Url = "?step="+i });
            }
            return Steps(steps, current, current, showLinks, stopLinksAtMaxComplete);
        }

        /// <summary>
        /// Render flow form steps.
        /// </summary>
        /// <param name="steps">A list of objects representing the steps in the form; object properties: {Name, Url}</param>
        /// <param name="current">The current step the user is on</param>
        /// <param name="showLinks">Whether or not to show links against the steps</param>
        /// <param name="stopLinksAtMaxComplete">Whether or not to show only links to completed steps</param>
        /// <returns></returns>
        public IHtmlString Steps(IList<Step> steps, int current, bool showLinks = false, bool stopLinksAtMaxComplete = false)
        {
            return Steps(steps, current, current, showLinks, stopLinksAtMaxComplete);
        }

        /// <summary>
        /// Render flow form steps.
        /// </summary>
        /// <param name="steps">A list of objects representing the steps in the form; object properties: {Name, Url}</param>
        /// <param name="current">The current step the user is on</param>
        /// <param name="maxComplete">The maximum step the user has completed thus far</param>
        /// <param name="showLinks">Whether or not to show links against the steps</param>
        /// <param name="stopLinksAtMaxComplete">Whether or not to show only links to completed steps</param>
        /// <returns></returns>
        public IHtmlString Steps(IList<Step> steps, int current, int maxComplete, bool showLinks = false, bool stopLinksAtMaxComplete = false)
        {
            return HelperDefinitions.Steps(steps, current, maxComplete, showLinks, stopLinksAtMaxComplete);
        }

        /// <summary>
        /// Render a flow form footer.
        /// </summary>
        /// <param name="area">The Area or Department within the University that is responsible for the form</param>
        /// <param name="id">The ID of the form as registered in the forms finder</param>
        /// <param name="date">The date the form was last registered</param>
        /// <returns>An IHtmlString object that will render the footer</returns>
        public IHtmlString Footer(string area = null, string id = null, string date = null)
        {
            return HelperDefinitions.Footer(area, id, date);
        }

        /// <summary>
        /// Render the start of a flow forms navigation area
        /// </summary>
        /// <remarks>
        /// Will render the end of the navigation after Dispose() is called.<br />
        /// Usually, you would use this within a using block and then use the returned navigation object to output buttons, e.g.:<br />
        /// using (var n = f.BeginNavigation() {<br />
        ///     &#160; &#160; @n.Submit(...)<br />
        ///     &#160; &#160; @n.Reset(...)<br />
        /// }
        /// </remarks>
        /// <returns>A FlowFormNavigation object that can be used to create submit and reset buttons</returns>
        public FlowFormNavigation BeginNavigation()
        {
            return new FlowFormNavigation(_htmlHelper.ViewContext.Writer);
        }

        /// <summary>
        /// Renders a flow form error message
        /// </summary>
        /// <param name="heading">The error heading</param>
        /// <param name="message">The error detail</param>
        /// <returns>A FlowFormMessage object (has a ToString() that prints "" so can safely have @ in Razor before this call)</returns>
        public FlowFormMessage ErrorMessage(string heading, IHtmlString message)
        {
            return new FlowFormMessage(_htmlHelper.ViewContext.Writer, MessageType.Error, heading, message);
        }

        /// <summary>
        /// Render the start of a flow form error message; output inner HTML then call .Dispose() on the return.
        /// </summary>
        /// <remarks>
        /// Will render the end of the message after Dispose() is called.<br />
        /// Usually, you would use this within a using block:<br />
        /// using (f.BeginErrorMessage("An error occurred") {<br />
        ///     &#160; &#160; &lt;p&gt;Please review the following errors and correct them:&lt;/p&gt;<br />
        ///     &#160; &#160; &lt;ul&gt;<br />
        /// &#160; &#160; &#160; &#160; &lt;li&gt;First error&lt;/li&gt;<br />
        /// &#160; &#160; &#160; &#160; &lt;li&gt;Second error&lt;/li&gt;<br />
        /// &#160; &#160; &lt;/ul&gt;<br />
        /// }
        /// </remarks>
        /// <param name="heading">The error heading</param>
        /// <returns>A FlowFormMessage object that doesn't need to be used (.Dispose() simply needs to be called after the contents are outputted)</returns>
        public FlowFormMessage BeginErrorMessage(string heading)
        {
            return new FlowFormMessage(_htmlHelper.ViewContext.Writer, MessageType.Error, heading);
        }

        /// <summary>
        /// Renders a flow form confirm message
        /// </summary>
        /// <param name="heading">The message heading</param>
        /// <param name="message">The message detail</param>
        /// <returns>A FlowFormMessage object (has a ToString() that prints "" so can safely have @ in Razor before this call)</returns>
        public FlowFormMessage ConfirmMessage(string heading, IHtmlString message)
        {
            return new FlowFormMessage(_htmlHelper.ViewContext.Writer, MessageType.Confirmation, heading, message);
        }

        /// <summary>
        /// Render the start of a flow form confirm message; output inner HTML then call .Dispose() on the return.
        /// </summary>
        /// <remarks>
        /// Will render the end of the message after Dispose() is called.<br />
        /// Usually, you would use this within a using block:<br />
        /// using (f.BeginConfirmMessage("Success") {<br />
        ///     &#160; &#160; &lt;p&gt;The item was successfully saved.:&lt;/p&gt;<br />
        /// }
        /// </remarks>
        /// <param name="heading">The error heading</param>
        /// <returns>A FlowFormMessage object that doesn't need to be used (.Dispose() simply needs to be called after the contents are outputted)</returns>
        public FlowFormMessage BeginConfirmMessage(string heading)
        {
            return new FlowFormMessage(_htmlHelper.ViewContext.Writer, MessageType.Confirmation, heading);
        }
    }

    public class Step
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public string Class { get; set; }
    }
}