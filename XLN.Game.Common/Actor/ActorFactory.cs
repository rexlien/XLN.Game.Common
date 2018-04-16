using System;
namespace XLN.Game.Common.Actor
{
    public abstract class ActorFactory
    {
        public ActorFactory()
        {
            
        }

        public virtual BaseActor CreateActor()
        {
            return null;
        }
        public virtual void CreateComponent(BaseActor actor)
        {
           
        }
    }
}
