using System;
using System.IO;
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

    }
}
