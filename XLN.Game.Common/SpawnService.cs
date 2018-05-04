using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using XLN.Game.Common.Actor;
using XLN.Game.Common.ComponentSystem;
using XLN.Game.Common.Utils;

namespace XLN.Game.Common
{
    [GuidAttribute("2982FAA2-EF4B-4586-8108-0237C0E9222A")]
    public class SpawnService : IService
    {
        public SpawnService()
        {
        }

        public enum TriggerDataType
        {
            TT_ITEM,
            TT_SKILL,

        }

        public struct SpawnParams
        {
            public string ActorType;
            public string AttributeComponent;
            public string AttributeKey;

            public string Slot;

            public BaseActor Owner;
            public bool AttachedOwner;

            public Vector3? InitPos;
            public Quaternion? InitRot;
            public Vector3? InitScale;

            //public Dictionary<string, object> OptionalParams;

        }

		public override bool OnInit()
		{
            m_ActorService = ServiceMgr.GetServiceMgr().GetService<ActorService>();
            return base.OnInit();
		}

		protected virtual BaseActor CreateActor()
        {
            return null;
        }

        public virtual BaseActor SpawnTrigger(SpawnParams spawnParam)
        {
            BaseActor actor = m_ActorService.AddActor(spawnParam.ActorType);
            //TransformComponent transform = actor.AddComponent<TransformComponent>(spawnParam.InitPos.HasValue ? spawnParam.InitPos.Value : Vector3.Zero, 
            //                                                                      spawnParam.InitRot.HasValue ? spawnParam.InitRot.Value : Quaternion.Identity, spawnParam.InitScale.HasValue ? spawnParam.InitScale.Value : Vector3.One);
            actor.AddComponent(spawnParam.AttributeComponent, spawnParam.AttributeKey);
            TriggerComponent trigger = actor.AddComponent<TriggerComponent>();
            trigger.Owner = spawnParam.Owner;

            MovementSystem movementSysten = m_ActorService.GetComponentSystem<MovementSystem>();
            movementSysten.Transform(actor, spawnParam.InitPos, spawnParam.InitRot, spawnParam.InitScale);



            return actor;
        }


        private ActorService m_ActorService;
    }
}
