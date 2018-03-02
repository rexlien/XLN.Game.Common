using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Game.Common
{
    public class ServiceMgr
    {

        public static ServiceMgr GetServiceMgr()
        {
            return s_ServiceMgr;
        }
      
        private static ServiceMgr s_ServiceMgr = new ServiceMgr();
        private Dictionary<Guid, IService> m_Services = new Dictionary<Guid, IService>();
       

        public ServiceMgr()
        {

        }

        public bool Init()
        {
            foreach (KeyValuePair<Guid, IService> s in m_Services)
            {
                if (!s.Value.OnInit())
                {
                    
                }

            }
            return true;
        }

        public bool Update(float delta)
        {
            foreach (KeyValuePair<Guid, IService> s in m_Services)
            {
                s.Value.OnUpdate(delta);
            }
            return true;
            
        }

        public bool PostUpdate(float delta)
        {
            foreach (KeyValuePair<Guid, IService> s in m_Services)
            {
                s.Value.OnPostUpdate(delta);
            }
            return true;
        }

        private T GetSubClassService<T>() where T : IService
        {
            var subclasses =
                   from assembly in AppDomain.CurrentDomain.GetAssemblies()
                   from type in assembly.GetTypes()
                   where type.IsSubclassOf(typeof(T))
                   select type;

            IService retService = null;
            foreach (Type t in subclasses)
            {
               
                if (m_Services.TryGetValue(t.GUID, out retService))
                    return retService as T;
            }

            return null;
        }

        public void RegisterService<T>(T service) where T : IService
        {
            Type t = typeof(T);
            //if (t.IsSealed)
            {
                IService retService = null;
                if (!m_Services.TryGetValue(t.GUID, out retService))
                {
                    m_Services.Add(typeof(T).GUID, service);

                }
            }
            
        }

        public T GetService<T>() where T : IService
        {
            Type serviceType = typeof(T);
            IService retService = null;
            if (m_Services.TryGetValue(serviceType.GUID, out retService))
            {
                return retService as T;
            }
            else
            {
                return GetSubClassService<T>();
            }
            
        }

        
    }
}
