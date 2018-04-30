using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using XLN.Game.Common;
using XLN.Game.Common.Config;
using XLN.Game.Unity.Extension;

namespace XLN.Game.Unity
{


    [GuidAttribute("E94F46A5-C062-4740-9B95-3312E2D19874")]
    public sealed class UnityResourceService : ResourceService
    {
      
        public UnityResourceService()
        {
            RegisterResourceCreator(".xml", new ResourceService.ResoruceCreator<UnityXMLResource>());
            RegisterResourceCreator(".json", new ResourceService.ResoruceCreator<UnityJsonResource>());
            RegisterResourceCreator(".unitybundle", new ResourceService.ResoruceCreator<UnityAssetBundle>());
        }

        protected override IResource GetResource(ResourcePath path)
        {
            
            ICacheable cacheable = null;
            if (m_Resources.TryGetValue(path.Path, out cacheable))
            {
                
                return (IResource)(cacheable);
            }
            else
            {
                LogService.Logger.Log(LogService.LogType.LT_DEBUG, "ResService Cache Missed");
                IResourceCreator creator = GetCreator(path.GetExt());
                IResource newRes = null;
                if (creator == null)
                {
                    newRes = new UnityResource<UnityEngine.Object>();
                }
                else
                    newRes = creator.Create();
                
                if(newRes != null)
                {
                    newRes.Load(path);
                    newRes = (IResource)(m_Resources.GetOrAdd(path.Path, newRes));
                }
                return newRes;
            }

        }


        protected override async Task<IResource> GetResourceAsync(ResourcePath path)
        {
            ICacheable cacheable = null;
            if (m_Resources.TryGetValue(path.Path, out cacheable))
            {   
                
                return (IResource)(cacheable);
            }
            else
            {
                LogService.Logger.Log(LogService.LogType.LT_DEBUG, "ResService Cache Missed");
                IResource newRes = null;
                IResourceCreator creator = GetCreator(path.GetExt());

                if (creator == null)
                {
                    newRes = new UnityResource<UnityEngine.Object>();
                }
                else
                    newRes = creator.Create();
                if (newRes != null)
                {
                    await newRes.LoadAsync(path);
                    newRes = (IResource)(m_Resources.GetOrAdd(path.Path, newRes));
                }
                return newRes;
            }


        }

        public override Task<T> Get<T>(ResourcePath path)
        {
            //Debug.Log("Get Resource Path: " + path.Path);
            //Unity resource can't async
            if (path.Type != ResourcePath.PathType.Resource)
            {
                var t = Task.Run(() =>
                {
                    IResource res = GetResource(path);
                    if (res != null)
                    {
                        return res.Deserialize<T>(path.SubFile);
                    }
                    return default(T);
                });

                return t;
            }
            else
            {
                IResource res = GetResource(path);
                if (res != null)
                {
                    return Task.FromResult(res.Deserialize<T>(path.SubFile));
                }
                else
                {
                    return Task.FromResult(default(T));
                }
               
            }
        }

        public override async Task<T> GetAsync<T>(ResourcePath path)
        {
           
            //Debug.Log("Get Resource Path: " + path.Path);
            IResource res = await GetResourceAsync(path);
            if(res != null)
            {
                return res.Deserialize<T>(path.SubFile);
            }

            return default(T);
        }

    }
}