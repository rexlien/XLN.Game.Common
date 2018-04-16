using UnityEngine;
using System;
using XLN.Game.Common;

namespace XLN.Game.Unity.Actor
{
    public class UnityActor : BaseActor
    {
        public UnityActor()
        {
            
        }

        public override void OnDestroy()
        {
            UnityEngine.Object.Destroy(m_GameObject);
        }

        public override void OnCreated()
        {
            base.OnCreated();
            ActorComponent behavior = m_GameObject.AddComponent<ActorComponent>();
            behavior.Actor = this;
            m_GameObject.name = GetType().ToString() + this.ID.ToString();
        }

        protected GameObject m_GameObject = new GameObject();
    }
}
