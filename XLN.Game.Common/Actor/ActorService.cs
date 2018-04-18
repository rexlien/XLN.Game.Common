//using System;
//using System.Runtime;
using Thrift.Protocol;
using Thrift.Transport;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using XLN.Game.Common.Actor;

namespace XLN.Game.Common
{
    [GuidAttribute("FE12B484-4013-4AF2-8261-1C541F4A03C4")]
    public class ActorService : IService
    {
        /*
        public class ActorAttribute
        {
            public int m_Type;
            public string m_Asset;

        }
*/
        public ActorService()
        {
            
        }

        public BaseActor CreateActor(ActorFactory factory)
        {
            BaseActor actor = factory.CreateActor();
            _AddActor(actor);
            return actor;
        }

        public void AddActor(BaseActor actor)
        {
            _AddActor(actor);
        }

        private void _AddActor(BaseActor actor)
        {
            actor.ID = System.Guid.NewGuid();
            actor.OnCreated();
            actor.ActorService = this;
            m_Actors.Add(actor.ID, actor); 
        }

        public void DestroyActor(BaseActor actor)
        {
            m_Actors.Remove(actor.ID);
            _DestoryActor(actor);
        }

        public void DestroyActor(System.Guid guid)
        {
            BaseActor actor = null;
            if(m_Actors.TryGetValue(guid, out actor))
            {
                m_Actors.Remove(guid);
                _DestoryActor(actor);
            }

        }

        private void _DestoryActor(BaseActor actor)
        {
            actor.OnDestroy();
        }

        public void AddComponent(IComponent component)
        {


        }

        public void RemoveComponent(BaseActor actor, IComponent component)
        {


        }


        protected Dictionary<System.Guid, BaseActor> m_Actors = new Dictionary<System.Guid, BaseActor>();
    }
}
