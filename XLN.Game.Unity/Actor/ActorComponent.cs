using System;
using UnityEngine;
using XLN.Game.Common;
using XLN.Game.Common.Actor;

namespace XLN.Game.Unity.Actor
{
    public class ActorComponent : BaseBehavior {
        public ActorComponent()
        {
            m_ActorService = ServiceMgr.GetServiceMgr().GetService<ActorService>();
        }

        private void OnDestroy()
        {
            //Actor.OnDestroy();
            //if(m_ActorService)

        }

        private ActorService m_ActorService;
    }
}
