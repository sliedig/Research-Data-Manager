using System;
using Urdms.Dmp.Models.Enums;

namespace Urdms.Dmp.Database.Entities.Components
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