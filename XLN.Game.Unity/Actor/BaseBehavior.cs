using UnityEngine;
using System.Collections;
using XLN.Game.Common;
using XLN.Game.Common.Actor;

namespace XLN.Game.Unity
{
    public class BaseBehavior : MonoBehaviour, IComponent
    {

        private BaseActor m_Actor;
        public BaseActor Actor
        {
            get
            {
                return m_Actor;
            }

            set
            {
                //TODO: clean old one's handler
                if (m_Actor != value)
                {
                    m_Actor = value;
                    m_Actor.OnEnterStateHandler += OnEnterState;
                    m_Actor.OnUpdateStateHandler += OnUpdateState;
                    m_Actor.OnLeaveStateHandler += OnLeaveState;
                    m_Actor.OnHurtHandler += OnHurt;
                    m_Actor.OnDeathHandler += OnDeath;
                }

            }
        }

        private void OnDestroy()
        {   
            if(Actor != null)
                Actor.RemoveComponent(this);
        }

        // Use this for initialization
        public virtual void Start()
        {

        }

        // Update is called once per frame
        public virtual void Update()
        {

        }



        public virtual void OnEnterState(State state)
        {

        }

        public virtual void OnUpdateState(float deltaTime, State state)
        {


        }

        public virtual void OnLeaveState(State state)
        {

        }

        public virtual void OnHurt(BaseActor source)
        {

        }

        public virtual void OnDeath(BaseActor source)
        {

        }

        public T Sibling<T>() where T : IComponent
        {
            throw new System.NotImplementedException();
        }

        public virtual void Init()
        {
            
        }

        public virtual void Destroy()
        {
           
        }
    }
}
