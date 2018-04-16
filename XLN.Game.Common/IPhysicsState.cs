using System.Collections;

public interface IPhysicsState
{

    void OnUpdateVelocity(float deltaTime);
    void OnResolveCollsion(float deltaTime);
    void OnUpdateObject(float deltaTime);

    
}
