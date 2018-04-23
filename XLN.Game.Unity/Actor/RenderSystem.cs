using System;
using XLN.Game.Common.Actor;

namespace XLN.Game.Unity.Actor
{
    public class RenderSystem : IComponentSystem
    {
        public RenderSystem()
        {
        }

        public override void OnActorAdded(Common.BaseActor actor)
        {
            base.OnActorAdded(actor);
        }
    }
}
