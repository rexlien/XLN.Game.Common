//using System;
//using System.Runtime;
using Thrift.Protocol;
using Thrift.Transport;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using XLN.Game.Common.Actor;
using System;
using XLN.Game.Common.Utils;
using System.Reflection;
using System.Linq;

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

        public override bool OnInit()
        {
            return base.OnInit();
        }

        public override bool OnUpdate(float delta)
        {
            foreach(var pair in m_Systems )
            {
                pair.Value.OnUpdate(delta);
            }

            return base.OnUpdate(delta);
        }


        public T AddActor<T>(params object[] param) where T : BaseActor
        {
            T actor = ClassUtils.CreateInstance<T>(param);
            _AddActor(actor);
            return actor;
        }

        public BaseActor AddActor(string actorType, params object[] param)
        {
            BaseActor actor = ClassUtils.CreateInstance<BaseActor>(actorType, param);
            _AddActor(actor);
            return actor;
        }

        private void _AddActor(BaseActor actor)
        {
            actor.ID = System.Guid.NewGuid();
            actor.ActorService = this;
            actor.OnCreated();
            m_Actors.Add(actor.ID, actor); 

            foreach(var pair in m_Systems)
            {
                pair.Value.OnActorAdded(actor);
            }
        }

        public void DestroyActor(BaseActor actor)
        {
            if(m_Actors.Remove(actor.ID))
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
            foreach (var pair in m_Systems)
            {
                pair.Value.OnActorRemoved(actor);
            }
            actor.OnDestroy();
        }

        public void AddComponent(IComponent component)
        {
            foreach (var pair in m_Systems)
            {
                pair.Value.OnComponentAdded(component);
            }

        }

        public void RemoveComponent(BaseActor actor, IComponent component)
        {

            foreach (var pair in m_Systems)
            {
                pair.Value.OnComponentRemoved(component);
            }
        }

        public void RegisterComponentSystem<T>(T service) where T : IComponentSystem
        {
            Type t = typeof(T);
            //if (t.IsSealed)
            {
                IComponentSystem retService = null;
                if (!m_Systems.TryGetValue(t.GUID, out retService))
                {
                    m_Systems.Add(typeof(T).GUID, service);

                }
            }

        }

        public void RegisterIComponentSystem(IComponentSystem service)
        {
            Guid guid = service.GetType().GUID;
            {
                IComponentSystem retService = null;
                if (!m_Systems.TryGetValue(guid, out retService))
                {
                    m_Systems.Add(guid, service);

                }
            }

        }

        public T GetComponentSystem<T>() where T : IComponentSystem
        {
            Type serviceType = typeof(T);
            IComponentSystem system = null;
            if (m_Systems.TryGetValue(serviceType.GUID, out system))
            {
                return system as T;
            }
            else
            {
                return GetSubClassSystem<T>();
            }
        }


        private static Assembly[] s_Assembly = AppDomain.CurrentDomain.GetAssemblies();
        private Dictionary<Type, IComponentSystem> m_SubSystemCache = new Dictionary<Type, IComponentSystem>();

        private T GetSubClassSystem<T>() where T : IService
        {
            IEnumerable<Type> subClases = null;
            IComponentSystem retSystem = null;
            m_SubSystemCache.TryGetValue(typeof(T), out retSystem);
            if (retSystem == null)
            {
                subClases =
                    from assembly in s_Assembly.Where(a => !a.GlobalAssemblyCache)
                    from type in assembly.GetTypes()
                    where type.IsSubclassOf(typeof(T))
                    select type;
            }
            else
            {
                return retSystem as T;
            }

            foreach (Type t in subClases)
            {
                if (m_Systems.TryGetValue(t.GUID, out retSystem))
                {
                    m_SubSystemCache.Add(typeof(T), retSystem);
                    return retSystem as T;
                }
            }

            return null;
        }



        protected Dictionary<System.Guid, BaseActor> m_Actors = new Dictionary<System.Guid, BaseActor>();
        protected Dictionary<System.Guid, IComponentSystem> m_Systems = new Dictionary<System.Guid, IComponentSystem>();
    }
}
