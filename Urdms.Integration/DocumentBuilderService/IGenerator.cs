namespace Urdms.DocumentBuilderService
{
    public interface IGenerator
    {
        void GeneratePdf(int id, string siteUri);
        void GenerateXml(int id, string siteUri);
    }
}