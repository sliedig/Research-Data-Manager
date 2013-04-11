using Urdms.DocumentBuilderService.Database.Entities;
using Urdms.DocumentBuilderService.Extensions;

namespace Urdms.DocumentBuilderService.Helpers
{
    public interface IXmlHelper
    {
        void Save(string filePath, DataManagementPlan entity);
    }

    public class XmlHelper : IXmlHelper
    {
        public void Save(string filePath, DataManagementPlan entity)
        {
            var doc = entity.ToXDocument();
            doc.Save(filePath);
        }
    }

    
}
