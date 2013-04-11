namespace Urdms.Dmp.Database.Entities.Components
{
    public class Confidentiality
    {
        public virtual bool IsSensitive { get; set; }
        public virtual string ConfidentialityComments { get; set; }
    }
}