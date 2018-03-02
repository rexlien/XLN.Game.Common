using System.Collections;

public enum StateResult
{
    Success,
    Running,
    Fail
};

public interface IState
{
    void OnEnterState();

    StateResult OnUpdateState(float deltaTime);

    void OnLeaveState();
   
}
