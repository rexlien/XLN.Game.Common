using UnityEngine;
using System.Collections;
using XLN.Game.Common;

namespace XLN.Game.Unity
{
    public class BaseBehavior : MonoBehaviour
    {

        /*private Unit m_Unit;
        public Unit Unit
        {
            get
            {
                return m_Unit;
            }

            set
            {
                //TODO: clean old one's handler
                if (m_Unit != value)
                {
                    m_Unit = value;
                    m_Unit.OnEnterStateHandler += OnEnterState;
                    m_Unit.OnUpdateStateHandler += OnUpdateState;
                    m_Unit.OnLeaveStateHandler += OnLeaveState;
                    m_Unit.OnHurtHandler += OnHurt;
                    m_Unit.OnDeathHandler += OnDeath;
                }

            }
        }
    */

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

        //public 


    }
}
