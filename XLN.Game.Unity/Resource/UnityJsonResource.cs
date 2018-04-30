using System;
using System.Threading.Tasks;
using UnityEngine;
using XLN.Game.Common;
using XLN.Game.Common.Resource;
using XLN.Game.Unity.Extension;

namespace XLN.Game.Unity
{
    public class UnityJsonResource : JsonResource
    {
        public UnityJsonResource()
        {
        }

        public override bool Load(ResourcePath path)
        {
            TextAsset asset = (TextAsset)Resources.Load(path.ResolveRealPath());
            if (!asset)
                return false;
            m_Resource = String.Copy(asset.text);
            return true;
        }

        public override async Task<bool> LoadAsync(ResourcePath path)
        {
            UnityEngine.Object obj 
             = await Resources.LoadAsync(path.ResolveRealPath());
            TextAsset asset = (TextAsset)obj;
            m_Resource = String.Copy(asset.text);
            return true;
        }

    }
}
