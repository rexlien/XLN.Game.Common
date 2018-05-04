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


        public Action<BaseActor> OnEnterIntercept;
        public Action<BaseActor> OnLeaveIntercept;


        public BaseActor()
        {
            m_ID = System.Guid.NewGuid();
        }

        public virtual void OnCreated()
        {
            
        }

        public virtual void OnDestroy()
        {
            foreach(var kv in m_Components)
            {
                m_ActorService.RemoveComponent(this, kv.Value);
                kv.Value.Destroy();
            }
            m_Components.Clear();
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

        public virtual void EnterIntercept(BaseActor source)
        {
            if(OnEnterIntercept != null)
            {
                OnEnterIntercept(source);
            }
        }

        public virtual void LeaveIntercept(BaseActor source)
        {
            if (OnLeaveIntercept != null)
            {
                OnLeaveIntercept(source);
            }
        }

        public virtual T AddComponent<T>(params object[] param) where T : IComponent
        {
            T comp = _CreateComponent<T>(param);
            comp.Actor = this;
            comp.Init();
            m_Components.Add(typeof(T).GUID, comp);
            if(m_ActorService != null)
            {
                m_ActorService.AddComponent(comp);
            }

            return comp;
        }

        public virtual IComponent AddComponent(string componentType, params object[] param)
        {
            IComponent comp = _CreateComponent(componentType, param);
            comp.Actor = this;
            comp.Init();
            m_Components.Add(comp.GetType().GUID, comp);
            if (m_ActorService != null)
            {
                m_ActorService.AddComponent(comp);
            }

            return comp;
        }

        public virtual void RemoveComponent(IComponent comp)
        {
            m_ActorService.RemoveComponent(this, comp);
            m_Components.Remove(comp.GetType().GUID);
            comp.Destroy();
        }

        protected virtual T _CreateComponent<T>(params object[] param) where T : IComponent
        {
            return ClassUtils.CreateInstance<T>(param);
        }

        protected virtual IComponent _CreateComponent(string componentType, params object[] param)
        {
            return ClassUtils.CreateInstance<IComponent>(componentType, param);
        }

        public virtual T GetComponent<T>()// where T : IComponent
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