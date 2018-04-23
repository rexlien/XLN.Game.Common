using System;
using System.Runtime.InteropServices;
using UnityEngine;
using XLN.Game.Common;
using XLN.Game.Common.Actor;

namespace XLN.Game.Unity.Actor
{
    [GuidAttribute("2B503DEC-40DD-4CC8-A1E9-F875D6E793F9")]
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
