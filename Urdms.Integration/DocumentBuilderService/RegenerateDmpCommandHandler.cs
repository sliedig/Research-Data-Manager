using Curtin.Urdms.DocumentBuilderService.Commands;
using NServiceBus;
using log4net;

namespace Curtin.Urdms.DocumentBuilderService
{
   public class RegenerateDmpCommandHandler : IHandleMessages<RegenerateDmpCommand>
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public RegenerateDmpCommandHandler() : this(new DmpGenerator()) { }

       public RegenerateDmpCommandHandler(IGenerator generator)
       {
           DocumentGenerator = generator;
       }

       protected IGenerator DocumentGenerator { get; private set; } 

        /// <summary>
        /// Handles the RegenerateDmpCommand message.
        /// </summary>
        /// <param name="message">RegenerateDmpCommand</param>
        public void Handle(RegenerateDmpCommand message)
        {
            Log.InfoFormat("[URMDS] Re-generating DMP PDF for reserach project \"{0}\".", message.ProjectId);
            DocumentGenerator.GeneratePdf(message.ProjectId, message.SiteUri);
            Log.Info("[URMDS] Completed DMP PDF re-generation.");

            Log.InfoFormat("[URMDS] Re-generating DMP XML for research project \"{0}\".", message.ProjectId);
            DocumentGenerator.GenerateXml(message.ProjectId, message.SiteUri);
            Log.Info("[URMDS] Completed DMP XML re-generation.");
        }
    }
}
