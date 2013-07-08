using System;
using NServiceBus;
using Urdms.Approvals.ApprovalService.Messages;
using Urdms.Approvals.VivoPublisher.Database.Repositories;
using Urdms.Approvals.VivoPublisher.Messages;
using NServiceBus.Logging;

namespace Urdms.Approvals.VivoPublisher
{
    public class ExportToVivoHandler : IHandleMessages<ExportToVivo>
    {
        private readonly IVivoDataCollectionRepository _vivoDataCollectionRepository;
        private readonly IDataCollectionRepository _dataCollectionRepository;
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public IBus Bus { get; set; }


        public ExportToVivoHandler()
            : this(new VivoDataCollectionRepository(), new DataCollectionRepository())
        {
        }

        public ExportToVivoHandler(IVivoDataCollectionRepository vivoDataCollectionRepository, IDataCollectionRepository dataCollectionRepository)
        {
            _vivoDataCollectionRepository = vivoDataCollectionRepository;
            _dataCollectionRepository = dataCollectionRepository;
        }

        public void Handle(ExportToVivo message)
        {
            try
            {
                Log.InfoFormat("[URDMS] Retrieving Data Collection info for id: {0}.", message.DataCollectionId);
                var dataCollection = _dataCollectionRepository.Get(message.DataCollectionId);

                Log.Info("[URDMS] Updating Vivo database.");
                _vivoDataCollectionRepository.Save(dataCollection);
                Log.Info("[URDMS] Vivo database update completed successfully.");

                // When complete reply to Approval Service
                Bus.Reply<ExportToVivoResponse>(m =>
                                                 {
                                                     m.RecordPublishedOn = DateTime.Now;
                                                     m.DataCollectionId = message.DataCollectionId;
                                                 });
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("[URDMS] Error publishing to Vivo. Exception: {0}", ex);
                throw;
            }

        }
    }
}