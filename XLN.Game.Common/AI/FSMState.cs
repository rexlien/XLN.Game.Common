using System.Collections;
using System.Collections.Generic;
using System;

public class FSMState : State
{
    private FSM m_FSM;
    private Dictionary<string, object> m_StateVariables = new Dictionary<string, object>();

    public FSMState()
    {

    }

    public FSM FSM
    {
        get
        {
            return m_FSM;
        }

        set
        {
            m_FSM = value;
        }
    }

    public Dictionary<string, object> StateVariables
    {
        get
        {
            return m_StateVariables;
        }

        set
        {
            m_StateVariables = value;
        }
    }

    public virtual Type TransitCondition(out object transtionParam)
    {
        transtionParam = null;
        return null;
    }

    public virtual void OnTransitState(FSMState from, FSMState to, object transitionParam)
    {

    }
   
   
}
