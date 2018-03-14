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

namespace XLN.Game.Unity
{


    [GuidAttribute("E94F46A5-C062-4740-9B95-3312E2D19874")]
    public sealed class UnityResourceService : ResourceService
    {
      
        protected override Resource<T> GetResource<T>(ResourcePath path)
        {
            
            ICacheable cacheable = null;
            if (m_Resources.TryGetValue(path.Path, out cacheable))
            {
                return (Resource<T>)(cacheable);
            }
            else
            {
                Resource<T> newRes = null;
                if (path.GetExt() == ".xml")
                {
                    newRes = new XMLUnityResource<T>();
                }
                else if (path.GetExt() == ".unitybundle")
                {
                    newRes = new UnityAssetBundle<T>();

                }
                else if (typeof(T) == typeof(UnityEngine.Object) || typeof(T).IsSubclassOf(typeof(UnityEngine.Object)))
                {
                    newRes = new UnityResource<T>();

                }
                if(newRes != null)
                {
                    newRes.Load(path);
                    newRes = (Resource<T>)(m_Resources.GetOrAdd(path.Path, newRes));
                }
                return newRes;
            }

        }


        protected override async Task<Resource<T>> GetResourceAsync<T>(ResourcePath path)
        {
            ICacheable cacheable = null;
            if (m_Resources.TryGetValue(path.Path, out cacheable))
            {
                return (Resource<T>)(cacheable);
            }
            else
            {
                Resource<T> newRes = null;
                if (path.GetExt() == ".xml")
                {
                    newRes = new XMLUnityResource<T>();
                }
                else if (path.GetExt() == ".unitybundle")
                {
                    newRes = new UnityAssetBundle<T>();

                }
                else if (typeof(T) == typeof(UnityEngine.Object) || typeof(T).IsSubclassOf(typeof(UnityEngine.Object)))
                {
                    newRes = new UnityResource<T>();

                }
                if (newRes != null)
                {
                    await newRes.LoadAsync(path);
                    newRes = (Resource<T>)(m_Resources.GetOrAdd(path.Path, newRes));
                }
                return newRes;
            }


        }

        public override Task<T> Get<T>(ResourcePath path)
        {
            Debug.Log("Get Resource Path: " + path.Path);
            //Unity resource can't async
            if (path.Type != ResourcePath.PathType.Resource)
            {
                var t = Task.Run(() =>
                {
                    Resource<T> res = GetResource<T>(path);
                    if (res != null)
                    {
                        return res.Deserialize(path.SubFile);
                    }
                    return default(T);
                });

                return t;
            }
            else
            {
                Resource<T> res = GetResource<T>(path);
                if (res != null)
                {
                    return Task.FromResult(res.Deserialize(path.SubFile));
                }
                else
                {
                    return Task.FromResult(default(T));
                }
               
            }
        }

        public override async Task<T> GetAsync<T>(ResourcePath path)
        {
           
            Debug.Log("Get Resource Path: " + path.Path);
            Resource<T> res = await GetResourceAsync<T>(path);
            if(res != null)
            {
                return res.Deserialize(path.SubFile);
            }

            return default(T);
        }

    }
}