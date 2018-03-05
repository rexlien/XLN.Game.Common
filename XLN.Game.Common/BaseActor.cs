using System.Collections;

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



        public virtual void OnCreate()
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
            if (OnHurtHandler != null)
                OnHurtHandler(source);

        }

        public abstract int GetID();
    }

}