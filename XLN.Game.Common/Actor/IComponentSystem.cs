using System;
namespace XLN.Game.Common.Actor
{
    //TODO: decouple from IService
    public abstract class IComponentSystem : IService
    {
        public IComponentSystem()
        {
            
        }

        public override bool OnInit()
        {
            m_ActorService = ServiceMgr.GetServiceMgr().GetService<ActorService>(); 
            return true;
        }

        public virtual void OnActorAdded(BaseActor actor)
        {
            
        }

        public virtual void OnActorRemoved(BaseActor actor)
        {
            
        }

        public virtual void OnComponentAdded(IComponent component)
        {
            
        }

        public virtual void OnComponentRemoved(IComponent component)
        {
            
        }

        protected ActorService m_ActorService;
    }
}
