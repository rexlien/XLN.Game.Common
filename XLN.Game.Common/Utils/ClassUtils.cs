using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;

namespace XLN.Game.Common.Utils
{
    public static class ClassUtils
    {

        private static ConcurrentDictionary<string, Type> s_TypeCache = new ConcurrentDictionary<string, Type>();
       
        public static Assembly GetAssemblyByName(string name)
        {
            
            return AppDomain.CurrentDomain.GetAssemblies().
                   SingleOrDefault(assembly => assembly.GetName().Name == name);
        }

        public static Type GetType(string classname)
        {
            Type t = Type.GetType(classname);
            if (t == null)
            {
                s_TypeCache.TryGetValue(classname, out t);
                if (t == null)
                {
                    var subclasses =
                        from assembly in AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.GlobalAssemblyCache)
                        from type in assembly.GetTypes()
                        where (type.FullName == classname)
                        select type;
                    t = subclasses.FirstOrDefault();
                    s_TypeCache.TryAdd(classname, t);
                }
            }
            return t;

        }

        public static Type GetType(string assemblyName, string classname)
        {
            
            Type t = Type.GetType(classname);
            if (t == null)
            {
                s_TypeCache.TryGetValue(classname, out t);
                if (t == null)
                {
                    Assembly assembly = GetAssemblyByName(assemblyName);
                    t = assembly.GetType(classname);
                    s_TypeCache.TryAdd(classname, t);
                }
            }
            return t;
        }

        public static T CreateInstance<T>(string assemblyname, string classname, params object[] args)
        {
            Type t = Type.GetType(classname);
            if (t == null)
            {
                s_TypeCache.TryGetValue(classname, out t);
                if (t == null)
                {
                    Assembly assembly = GetAssemblyByName(assemblyname);
                    t = assembly.GetType(classname);
                    s_TypeCache.TryAdd(classname, t);
                }
            }

            //ObjectHandle objHandle = Activator.CreateInstance(assemblyname, classname);
            //if (objHandle != null)
            //{
            //    T ret = (T)objHandle.Unwrap();
            //    return ret;
            //}
            return (T)Activator.CreateInstance(t, args);

        }

        public static T CreateInstance<T>(string classname, params object[] args)
        {
            Type t = GetType(classname);
            if (t == null)
                return default(T);

            return (T)Activator.CreateInstance(t, args);
        }

        public static T CreateInstance<T>(params object[] args)
        {
            Type t = typeof(T);
            if(t != null)
            {
                return (T)Activator.CreateInstance(t, args);

            }
            return default(T);

        }


    }
}
