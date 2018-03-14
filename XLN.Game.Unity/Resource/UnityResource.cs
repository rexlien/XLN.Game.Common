using System;
using System.IO;
using UnityEngine;
using XLN.Game.Common;
using XLN.Game.Unity.Extension;

namespace XLN.Game.Unity
{
    public class UnityResource<T> : Resource<T>
    {
        public override T Deserialize(string key)
        {
            if (m_Resource is UnityEngine.GameObject)
            {
                object ret = (object)m_Resource;//UnityEngine.Object.Instantiate((UnityEngine.Object)(object)m_Resource);
                return (T)(ret);
            }
            else
                return m_Resource;
        }

        public override T Deserialize(Stream stream)
        {
            throw new NotImplementedException();
        }

        public override bool Load(ResourcePath path)
        {
            if (path.Type == ResourcePath.PathType.Resource)
                m_Resource = (T)(object)Resources.Load(path.ResolveRealPath());
          
            return true;
        }

        protected override Stream GetStream()
        {
            throw new NotImplementedException();
        }



        private T m_Resource;

    }
}
