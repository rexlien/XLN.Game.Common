using System;
namespace XLN.Game.Common.Actor
{
    public interface IComponent
    {
        T Sibling<T>() where T : IComponent;
        BaseActor Actor { get; set; }
    }

    public class BaseComponent : IComponent
    {
        public T Sibling<T>() where T : IComponent
        {
            return m_Actor.GetComponent<T>();
        }

        private BaseActor m_Actor;

        BaseActor IComponent.Actor { get => m_Actor; set => m_Actor = value; }
    }
}
