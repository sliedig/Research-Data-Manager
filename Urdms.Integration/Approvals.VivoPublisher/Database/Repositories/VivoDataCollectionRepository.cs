using Urdms.Approvals.VivoPublisher.Database.Entities;

namespace Urdms.Approvals.VivoPublisher.Database.Repositories
{
    public interface IVivoDataCollectionRepository
    {
        void Save(DataCollection dataCollection);
    }

    public class VivoDataCollectionRepository : IVivoDataCollectionRepository
    {
        /// <summary>
        /// Writes the specified Data Collection to the Vivo database.
        /// </summary>
        /// <param name="dataCollection">The data collection.</param>
        public void Save(DataCollection dataCollection)
        {
			// TODO: Implement this, save somewhere you can push data out to ANDS from
        }
    }
}
