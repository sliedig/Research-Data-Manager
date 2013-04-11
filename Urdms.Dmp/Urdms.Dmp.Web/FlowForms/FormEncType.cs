using System.ComponentModel;

namespace Urdms.Dmp.Web.FlowForms
{
    ///<summary>
    /// Representation of the different form encoding types.
    /// Use StringEnum.GetStringValue() or .ToDescription() to get the enc type for output.
    ///</summary>
    public enum FormEncType
    {
        ///<summary>
        /// URL encoded
        ///</summary>
        [Description("application/x-www-form-urlencoded")]
        UrlEncoded,
        ///<summary>
        /// Multipart
        ///</summary>
        [Description("multipart/form-data")]
        Multipart,
        ///<summary>
        /// Plain text
        ///</summary>
        [Description("text/plain")]
        Plain
    }
}