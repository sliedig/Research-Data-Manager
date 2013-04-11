namespace Urdms.Dmp.Database.Entities
{
    public class DataCollectionSocioEconomicObjective : ClassificationBase
    {
        public virtual SocioEconomicObjective SocioEconomicObjective
        {
            get { return Code as SocioEconomicObjective; }
            set { Code = value; }
        }
    }
}

