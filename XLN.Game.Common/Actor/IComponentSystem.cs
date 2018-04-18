using System;
namespace XLN.Game.Common.Actor
{
    //TODO: decouple from IService
    public class IComponentSystem : IService
    {
        public IComponentSystem()
        {
            
        }

        public override bool OnInit()
        {
            m_ActorService = ServiceMgr.GetServiceMgr().GetService<ActorService>(); 
            return true;
        }
        protected ActorService m_ActorService;
    }
}
