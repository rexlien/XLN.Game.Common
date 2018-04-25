using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;

namespace XLN.Game.Common.Utils
{
    public static class ClassUtils
    {

        private static Dictionary<string, Type> s_TypeCache = new Dictionary<string, Type>();
       
        public static Assembly GetAssemblyByName(string name)
        {
            return AppDomain.CurrentDomain.GetAssemblies().
                   SingleOrDefault(assembly => assembly.GetName().Name == name);
        }

        public static Type GetType<T>()
        {
            var subclasses =
                from assembly in AppDomain.CurrentDomain.GetAssemblies().Where(a=>!a.GlobalAssemblyCache)
                   from type in assembly.GetTypes()
                   where (type == typeof(T))
                   select type;
            return subclasses.FirstOrDefault();

        }

        public static T CreateInstance<T>(string assemblyname, string classname)
        {
            Type t = Type.GetType(classname);
            if (t == null)
            {
                Assembly assembly = GetAssemblyByName(assemblyname);
                t = assembly.GetType(classname);
            }
            ObjectHandle objHandle = Activator.CreateInstance(assemblyname, classname);
            if (objHandle != null)
            {
                T ret = (T)objHandle.Unwrap();
                return ret;
            }
            return default(T);

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
