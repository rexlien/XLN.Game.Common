using System;
namespace XLN.Game.Common.Actor
{
    public interface IComponent
    {
        T Sibling<T>() where T : IComponent;
        BaseActor Actor { get; set; }

        void Init();
        void Destroy();
    }


}
