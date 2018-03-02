using System.Collections;

public class State : IState
{

    protected BaseActor m_Actor;

    public BaseActor Actor
    {
        get
        {
            return m_Actor;
        }

        set
        {
            m_Actor = value;
            OnActorChagned();
        }
    }

    public State()
    {

    }

    public virtual void OnEnterState()
    {
        if(m_Actor != null)
            m_Actor.EnterState(this);
    }

    public virtual StateResult OnUpdateState(float deltaTime)
    {
        if(m_Actor != null)
            m_Actor.UpdateState(deltaTime, this);

        return StateResult.Success;
    }

    public virtual void OnLeaveState()
    {
        if(m_Actor != null)
            m_Actor.LeaveState(this);
    }

    protected virtual void OnActorChagned()
    {

    }

}
