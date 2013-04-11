
namespace Urdms.Dmp.Database.Entities
{
    public class ProjectSocioEconomicObjective : ClassificationBase
    {
        public virtual SocioEconomicObjective SocioEconomicObjective
        {
            get { return Code as SocioEconomicObjective; }
            set { Code = value; }
        }
    }
}

