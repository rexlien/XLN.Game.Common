using UnityEngine;
using System.Collections;
using XLN.Game.Common;

namespace XLN.Game.Unity
{
    public class BaseBehavior : MonoBehaviour
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


    }
}
