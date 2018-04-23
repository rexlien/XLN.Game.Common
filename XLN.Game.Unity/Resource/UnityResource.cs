using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using XLN.Game.Common;
using XLN.Game.Unity.Extension;

namespace XLN.Game.Unity
{
    public class UnityResource<T> : Resource<T>
    {
        public override R Deserialize<R>(string key)
        {
            object ret = (object)m_Resource;
            return (R)(ret);

        }

        public override bool Load(ResourcePath path)
        {
            if (path.Type == ResourcePath.PathType.Resource)
                m_Resource = (T)(object)Resources.Load(path.ResolveRealPath());
          
            return true;
        }

        public override async Task<bool> LoadAsync(ResourcePath path)
        {

            if (path.Type == ResourcePath.PathType.Resource)
            {
                UnityEngine.Object obj = await Resources.LoadAsync(path.ResolveRealPath());
                m_Resource = (T)(object)obj;
                return true;
            }

            return false;
        }

    }
}
