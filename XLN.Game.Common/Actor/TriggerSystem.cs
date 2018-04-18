using System;
using System.Collections.Generic;

namespace XLN.Game.Common.Actor
{
    public class TriggerSystem : IComponentSystem
    {
        public TriggerSystem()
        {
            
        }


        public enum TriggerDataType
        {
            TT_ITEM,
            TT_SKILL,

        }

        public struct SpawnParams
        {
            public int TriggerID;

            public TriggerDataType DataSourceType;
            public int DataSourceID;

            public string Slot;

            public bool AttachSpawner;
            public Dictionary<string, object> OptionalParams;

        }

        public virtual void SpawnTrigger(SpawnParams spawnParam)
        {
            
        }
    }
}
