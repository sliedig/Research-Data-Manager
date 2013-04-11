using System.IO;
using Urdms.DocumentBuilderService.Database.Repositories;
using Urdms.DocumentBuilderService.Helpers;

namespace Urdms.DocumentBuilderService
{
    public class DmpGenerator : IGenerator
    {
        public DmpGenerator()
            : this(new SharePointHelper(), new DataManagementPlanRepository(), new PdfHelper(), new GeneratorHelper(), new XmlHelper())
        {

        }

        public DmpGenerator(ISharePointHelper sharePointHelper, IDataManagementPlanRepository repository, IPdfHelper pdfHelper, IGeneratorHelper generatorHelper, IXmlHelper xmlHelper)
        {
            SharePointHelper = sharePointHelper;
            Repository = repository;
            PdfHelper = pdfHelper;
            GeneratorHelper = generatorHelper;
            XmlHelper = xmlHelper;
        }


        protected ISharePointHelper SharePointHelper { get; private set; }
        protected IDataManagementPlanRepository Repository { get; private set; }
        protected IPdfHelper PdfHelper { get; private set; }
        protected IGeneratorHelper GeneratorHelper { get; private set; }
        protected IXmlHelper XmlHelper { get; private set; }

        public void GeneratePdf(int id, string siteUri)
        {
            var filePath = GeneratorHelper.CreateFilePath("pdf");
            var dataManagementPlan = Repository.GetDataManagementPlanByProjectId(id);

            //PdfHelper.GeneratePdf("DmpPdfTemplate", dataManagementPlan).Save(filePath);	// TODO: Plug in ABC pdf or other PDF generator
            //SharePointHelper.UploadDocumentToSharePoint(filePath, siteUri);

            // Cleanup
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public void GenerateXml(int id, string siteUri)
        {
            var filePath = GeneratorHelper.CreateFilePath("xml");
            var dataManagementPlan = Repository.GetDataManagementPlanByProjectId(id);
            XmlHelper.Save(filePath, dataManagementPlan);
            //SharePointHelper.UploadDocumentToSharePoint(filePath, siteUri);

            // CleanUp
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
