using Urdms.Dmp.Models.Enums;

namespace Urdms.Dmp.Models
{
    public abstract class ProjectFunderViewModel
    {
        public abstract bool IsFunded { get; set; }
        public abstract Funder Funder { get; }
        public abstract string GrantNumber { get; set; }
    }
}