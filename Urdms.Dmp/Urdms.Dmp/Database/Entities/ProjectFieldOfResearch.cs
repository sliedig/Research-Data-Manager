namespace Urdms.Dmp.Database.Entities
{
    public class ProjectFieldOfResearch : ClassificationBase
    {
        public virtual FieldOfResearch FieldOfResearch
        {
            get { return Code as FieldOfResearch; }
            set { Code = value; }
        }
    }
}

