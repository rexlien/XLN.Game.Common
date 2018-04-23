using System;
namespace XLN.Game.Common.Actor
{
    public class BaseComponent : IComponent
    {
        public T Sibling<T>() where T : IComponent
        {
            return m_Actor.GetComponent<T>();
        }

        private BaseActor m_Actor;

        public BaseActor Actor { get => m_Actor; set => m_Actor = value; }
        public virtual void Init() { }

        public virtual void Destroy()
        {
            
        }
    }
}
