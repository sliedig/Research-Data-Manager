using System;
using Urdms.Approvals.VivoPublisher.Database.Enums;

namespace Urdms.Approvals.VivoPublisher.Database.Entities.Components
{
    public class DataCollectionState
    {
        public virtual DataCollectionStatus State { get; private set; }
        public virtual DateTime StateChangedOn { get; private set; }

        public DataCollectionState()
        {
        }

        public DataCollectionState(DataCollectionStatus state, DateTime stateChanged)
        {
            State = state;
            StateChangedOn = stateChanged;
        }
    }
}