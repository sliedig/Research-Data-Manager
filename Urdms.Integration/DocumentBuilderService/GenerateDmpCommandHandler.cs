using System;
using NServiceBus;
using Urdms.DocumentBuilderService.Commands;
using Urdms.DocumentBuilderService.Database.Repositories;
using Urdms.DocumentBuilderService.Models.Enums;
using log4net;

namespace Urdms.DocumentBuilderService
{
    public class GenerateDmpCommandHandler : IHandleMessages<GenerateDmpCommand>
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected IGenerator DocumentGenerator { get; private set; }
        protected IProjectRepository ProjectRepository { get; private set; }

        public GenerateDmpCommandHandler() : this(new DmpGenerator(), new ProjectRepository()) { }

        public GenerateDmpCommandHandler(IGenerator documentGenerator, IProjectRepository projectRepository)
        {
            DocumentGenerator = documentGenerator;
            ProjectRepository = projectRepository;
        }

        #region Implementation of IMessageHandler<GenerateDmpCommand>

        public void Handle(GenerateDmpCommand message)
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
                    DocumentGenerator.GeneratePdf(message.ProjectId, message.SiteUrl);
                    Log.Info("[URMDS] Completed DMP PDF generation.");

                    Log.InfoFormat("[URMDS] Generating DMP XML for research project \"{0}\".", message.ProjectId);
                    DocumentGenerator.GenerateXml(message.ProjectId, message.SiteUrl);
                    Log.Info("[URMDS] Completed DMP XML generation.");

                }
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("Message:\n{0}\nStackTrace:\n{1}", ex.Message, ex.StackTrace));
                throw;
            }
        }

        #endregion
    }
}