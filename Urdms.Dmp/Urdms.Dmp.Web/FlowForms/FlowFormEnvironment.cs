using System.ComponentModel;

namespace Urdms.Dmp.Web.FlowForms
{
    public enum FlowFormEnvironment
    {
        [Description("")]
        Production,
        [Description("test.")]
        Test,
        [Description("dev.")]
        Development
    }
}