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
            ActorComponent behavior = AddComponent<ActorComponent>();
            m_GameObject.name = GetType().ToString() + this.ID.ToString();
        }

        protected override T _CreateComponent<T>()
        {
            
            if(typeof(T).IsSubclassOf(typeof(BaseBehavior)))
            {
                T behavior = (T)(object)m_GameObject.AddComponent(typeof(T));//AddComponent<T>();
                return behavior;
            }
            else
            {
                return base._CreateComponent<T>();
            }

        }

        protected GameObject m_GameObject = new GameObject();
    }
}
