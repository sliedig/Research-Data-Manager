using System;
using Curtin.Urdms.DocumentBuilderService.Database.Repositories;
using Curtin.Urdms.DocumentBuilderService.Models.Enums;
using Curtin.Urdms.ProvisioningService.Events;
using NServiceBus;
using log4net;

namespace Curtin.Urdms.DocumentBuilderService
{
    public class SiteProvisionedHandler : IHandleMessages<SiteProvisioned>
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected IGenerator DocumentGenerator { get; private set; }
        protected IProjectRepository ProjectRepository { get; private set; }

        public SiteProvisionedHandler() : this(new DmpGenerator(), new ProjectRepository()) { }

        public SiteProvisionedHandler(IGenerator documentGenerator, IProjectRepository projectRepository)
        {
            DocumentGenerator = documentGenerator;
            ProjectRepository = projectRepository;
        }

        public void Handle(SiteProvisioned message)
        {
            try
            {
                var entity = ProjectRepository.Get(message.ProjectId);
                if (entity == null)
                {
                    throw new ApplicationException(string.Format("[URMDS] Project {0} Not Found In The Database", message.ProjectId));
                }

                if (entity.SourceProjectType != SourceProjectType.DEPOSIT)
                {
                    Log.InfoFormat("[URMDS] Generating DMP PDF for reserach project \"{0}\".", message.ProjectId);
                    DocumentGenerator.GeneratePdf(message.ProjectId, message.SiteUri);
                    Log.Info("[URMDS] Completed DMP PDF generation.");

                    Log.InfoFormat("[URMDS] Generating DMP XML for research project \"{0}\".", message.ProjectId);                   
                    DocumentGenerator.GenerateXml(message.ProjectId, message.SiteUri);
                    Log.Info("[URMDS] Completed DMP XML generation.");

                }
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("Message:\n{0}\nStackTrace:\n{1}", ex.Message, ex.StackTrace));
                throw;
            }
        }
    }
}
