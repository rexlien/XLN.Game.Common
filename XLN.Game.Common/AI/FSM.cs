
using System.Collections;
using System.Collections.Generic;
using System;
using XLN.Game.Common;

public class FSM : FSMState, IPhysicsState
{

    protected Dictionary<Type, FSMState> m_States = new Dictionary<Type, FSMState>();
    protected FSMState m_StartState;
    protected FSMState m_CurState;

    public FSM(FSMState startState)
    {
        AddState(startState);
        m_StartState = startState;
    }

    public void AddState(FSMState state)
    {
        state.FSM = this;
        m_States.Add(state.GetType(), state);
        
    }

    //public void AddTransition(State from, State to)

    public void TransitState<T>()
    {
        FSMState state;

        if(m_States.TryGetValue(typeof(T), out state))
        {

        }
        else
        {
            //wranning log
        }

    }

    public void TransitState(Type type, object transitionParam)
    {
        FSMState state;

        if (m_States.TryGetValue(type, out state))
        {
            DoTransit(state, transitionParam);
        }
        else
        {
            //wranning log
        }
    }

    protected void DoTransit(FSMState state, object transitionParam)
    {
        if (m_CurState != null)
        {
            m_CurState.OnLeaveState();
        }

        state.OnTransitState(m_CurState, state, transitionParam);
        m_CurState = state;

        if (m_CurState != null)
            m_CurState.OnEnterState();
    }

    public override void OnEnterState()
    {
        DoTransit(m_StartState, null);
        base.OnEnterState();

    }

    public override StateResult OnUpdateState(float deltaTime)
    { 
        if (m_CurState == null)
            return StateResult.Fail;

        m_CurState.OnUpdateState(deltaTime);

        //check transtion condition
        object transitionParam = null;
        Type stateType = m_CurState.TransitCondition(out transitionParam);
        if(stateType != null)
        {
            TransitState(stateType, transitionParam);
        }

        return base.OnUpdateState(deltaTime);

    }

    public override void OnLeaveState()
    {
        if (m_CurState != null)
            m_CurState.OnLeaveState();

        base.OnLeaveState();
    }
    /*
    public FSMState GetCurState()
    {
        return m_CurState;
    }
    */
    public void OnUpdateVelocity(float deltaTime)
    {
        FSMState curState = m_CurState;
        if (curState is IPhysicsState)
        {
            IPhysicsState physicsState = (IPhysicsState)curState;
            physicsState.OnUpdateVelocity(deltaTime);
        }
    }

    public void OnResolveCollsion(float deltaTime)
    {
        FSMState curState = m_CurState;
        if (curState is IPhysicsState)
        {
            IPhysicsState physicsState = (IPhysicsState)curState;
            physicsState.OnResolveCollsion(deltaTime);
        }
    }

    public void OnUpdateObject(float deltaTime)
    {
        FSMState curState = m_CurState;
        if (curState is IPhysicsState)
        {
            IPhysicsState physicsState = (IPhysicsState)curState;
            physicsState.OnUpdateObject(deltaTime);
        }

    }

    public void Start()
    {
        if(m_CurState == null)
        {
            OnEnterState();
        }
    }

    public bool IsStarted()
    {
        return (m_CurState != null);
    }
}
