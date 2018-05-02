using System;
using System.Numerics;
using XLN.Game.Common;
using XLN.Game.Common.Actor;
using XLN.Game.Unity.Extension;

namespace XLN.Game.Unity.Actor
{
    public class MovementSystem : XLN.Game.Common.ComponentSystem.MovementSystem
    {
        public MovementSystem()
        {


        }

        public override void Transform(BaseActor actor, Vector3? pos, Quaternion? rot, Vector3? scale)
        {
            TransformComponent transform = actor.GetComponent<TransformComponent>();
            if (transform != null)
            {
                if(pos != null)
                    transform.Position = pos.Value;
                if(rot != null)
                    transform.Rotation = rot.Value;
                if(scale != null)
                    transform.Scale = scale.Value;

            }

            RenderComponent renderComponent = transform.Sibling<RenderComponent>();//actor.GetComponent<RenderComponent>();
            //if(renderComponent != null)
            {
                SyncRenderComponent(renderComponent, transform);
            }
            //TODO:
            //UnityActor unityActor = (UnityActor)actor;
            //unityActor.SyncUnityPos();
        }

        private void SyncRenderComponent(RenderComponent renderComponent, TransformComponent transformComponent)
        {
            renderComponent.UnityPos = transformComponent.Position.ToUnityVec3();
            renderComponent.UnityRot = transformComponent.Rotation.ToUnityQuatenion();
            renderComponent.UnityScale = transformComponent.Scale.ToUnityVec3();
        }


    }
}
