using System;
using System.Collections;
using System.Collections.Generic;
using XLN.Game.Common.Actor;
using XLN.Game.Common.Utils;

namespace XLN.Game.Common
{

    public abstract class BaseActor
    {

        public delegate void HurtDelegate(BaseActor source);
        public event HurtDelegate OnHurtHandler;

        public delegate void OnDeathDelgate(BaseActor source);
        public event OnDeathDelgate OnDeathHandler;

        public delegate void StateChangeDelegate(State state);
        public delegate void StateUpdateDelegate(float deltaTime, State state);
        public event StateChangeDelegate OnEnterStateHandler;
        public event StateChangeDelegate OnLeaveStateHandler;
        public event StateUpdateDelegate OnUpdateStateHandler;


        public virtual void OnCreated()
        {

        }

        public virtual void OnDestroy()
        {

        }

        public virtual void EnterState(State state)
        {
            if (OnEnterStateHandler != null)
                OnEnterStateHandler(state);
        }

        public virtual void UpdateState(float deltaTime, State state)
        {
            if (OnUpdateStateHandler != null)
                OnUpdateStateHandler(deltaTime, state);
        }

        public virtual void LeaveState(State state)
        {
            if (OnLeaveStateHandler != null)
                OnLeaveStateHandler(state);
        }

        protected virtual void OnHurt(BaseActor source)
        {
            if (OnHurtHandler != null)
                OnHurtHandler(source);

        }

        protected virtual void OnDeath(BaseActor source)
        {
            if (OnDeathHandler != null)
                OnDeathHandler(source);

        }

        public virtual T AddComponent<T>() where T : IComponent
        {
            T comp = _CreateComponent<T>();
            comp.Actor = this;
                //ClassUtils.CreateInstance<T>();
            if(m_ActorService != null)
            {
                m_Components.Add(typeof(T).GUID, comp);
                m_ActorService.AddComponent(comp);
            }

            return comp;
        }

        public virtual void RemoveComponent(IComponent comp)
        {
            m_ActorService.RemoveComponent(this, comp);
            m_Components.Remove(comp.GetType().GUID);
        }

        protected virtual T _CreateComponent<T>() where T : IComponent
        {
            return ClassUtils.CreateInstance<T>();
        }

        public T GetComponent<T>() where T : IComponent
        {
            IComponent comp = null;
            m_Components.TryGetValue(typeof(T).GUID, out comp);
            return (T)comp;
        }

        private Dictionary<Guid, IComponent> m_Components = new Dictionary<Guid, IComponent>();
       

        private System.Guid m_ID;
        public Guid ID { get => m_ID; set => m_ID = value; }
        public ActorService ActorService { get => m_ActorService; set => m_ActorService = value; }

        private ActorService m_ActorService;
    }

}