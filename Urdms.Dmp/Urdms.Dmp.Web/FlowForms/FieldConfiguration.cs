using System;
using System.Web;
using System.Web.Mvc;

namespace Urdms.Dmp.Web.FlowForms
{
    /// <summary>
    /// Configuration object to supply properties for a flow form field.
    /// </summary>
    public class FieldConfiguration
    {
        public FieldConfiguration()
        {
            HideTip = true;
            DisplayFieldName = true;
        }

        /// <summary>
        /// A string to show as a tip for the field.
        /// </summary>
        public string Tip { get; set; }

        /// <summary>
        /// A HTML string to display as a hint for the field and any children it has.
        /// </summary>
        public IHtmlString Hint { get; set; }

        /// <summary>
        /// Any html attributes to add to the field element.
        /// </summary>
        /// <remarks>
        /// You can use an anonymous object for this e.g. new { size = "20", onclick="return false;", ... }
        /// </remarks>
        public object HtmlAttributes { get; set; }

        /// <summary>
        /// Used for select boxes to specify a default value.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Whether or not the tip should be hidden by JavaScript; true by default.
        /// </summary>
        public bool HideTip { get; set; }

        /// <summary>
        /// Any classes to add to the hint, e.g. 'plain', 'wide', 'narrow'.
        /// </summary>
        public string HintClass { get; set; }

        /// <summary>
        /// A list of values to exclude from a list (if using an enumeration).
        /// </summary>
        public Enum[] Exclude { get; set; }

        /// <summary>
        /// Any classes to add to the parent &lt;dt> and &lt;dd>
        /// </summary>
        public string ParentClass { get; set; }

        /// <summary>
        /// Override the default element type
        /// </summary>
        public ElementType? As { get; set; }

        /// <summary>
        /// Provide the list for Select/Radioboxes/Checkboxes
        /// </summary>
        public MultiSelectList PossibleValues { get; set; }

        /// <summary>
        /// String to print directly before the field
        /// </summary>
        public string Before { get; set; }

        /// <summary>
        /// String to print directly after the field
        /// </summary>
        public string After { get; set; }

        /// <summary>
        /// Control whether the field name (dt) is displayed. Default is true
        /// </summary>
        public bool DisplayFieldName { get; set; }

        /// <summary>
        /// Overide the content of the field name (dt)
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Print the value as text/readonly (no input element)
        /// </summary>
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Boolean that hides the field if null (only applies if ReadOnly = true)
        /// </summary>
        public bool HideIfNull { get; set; }
    }
}
