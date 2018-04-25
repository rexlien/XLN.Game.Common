using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Text;
using XLN.Game.Common.Config;

namespace XLN.Game.Common
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

        public bool OnDestroy()
        {
            foreach (KeyValuePair<Guid, IService> s in m_Services)
            {
                s.Value.OnDestroy();
            }
            return true;

        }

        public void OnEvent(int eventID)
        {
            foreach (KeyValuePair<Guid, IService> s in m_Services)
            {
                s.Value.OnEvent(eventID);
            }
        }

        public bool PostUpdate(float delta)
        {
            foreach (KeyValuePair<Guid, IService> s in m_Services)
            {
                s.Value.OnPostUpdate(delta);
            }
            return true;
        }

        private static Assembly[] s_Assembly = AppDomain.CurrentDomain.GetAssemblies();
        private Dictionary<Type, IService> m_SubClassCache = new Dictionary<Type, IService>();

        private T GetSubClassService<T>() where T : IService
        {
            IEnumerable<Type> subClases = null;
            IService retService = null;
            m_SubClassCache.TryGetValue(typeof(T), out retService);
            if (retService == null)
            {
                subClases =
                    from assembly in s_Assembly.Where(a => !a.GlobalAssemblyCache)
                from type in assembly.GetTypes()
                where type.IsSubclassOf(typeof(T))
                select type;
            }
            else
            {
                return retService as T;
            }

            foreach (Type t in subClases)
            {
                if (m_Services.TryGetValue(t.GUID, out retService))
                {   m_SubClassCache.Add(typeof(T), retService);
                    return retService as T;
                }
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
                else
                {
                    LogService.Logger.Log(LogService.LogType.LT_ERROR, "Service id duplicated");
                }
            }
            
        }

        public void RegisterIService(IService service) 
        {
            
            Guid guid = service.GetType().GUID;
           
            {
                IService retService = null;
                if (!m_Services.TryGetValue(guid, out retService))
                {
                    m_Services.Add(guid, service);

                }
                else
                {
                    LogService.Logger.Log(LogService.LogType.LT_ERROR, "Service id duplicated");
                }
            }

        }


        static Assembly GetAssemblyByName(string name)
        {
            return AppDomain.CurrentDomain.GetAssemblies().
                   SingleOrDefault(assembly => assembly.GetName().Name == name);
        }

        public void RegisterService(string assemblyName, string classname)
        {

            Type t = Type.GetType(classname);
            if (t == null)
            { 
                Assembly assembly = GetAssemblyByName(assemblyName);
                t = assembly.GetType(classname);
            }
            ObjectHandle objHandle = Activator.CreateInstance(assemblyName, classname);
            if(objHandle != null)
            {
                IService service = (IService)objHandle.Unwrap();
                m_Services.Add(t.GUID, service);    
            }
            else
            {
                if(LogService.Logger != null)
                {
                    LogService.Logger.Log(LogService.LogType.LT_ERROR, "Service can't create:" + classname);
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

        public void InjectService(AppConfig config)
        {
            foreach(AppConfig.Service service in config.AppServices.ServiceItems)
            {
                string className = service.Class;
                string assemblyName = service.AssemblyName;

                RegisterService(assemblyName, className);
            }
                  
        }


        
    }
}
