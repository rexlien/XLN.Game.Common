using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using XLN.Game.Common;
using XLN.Game.Unity.Extension;

namespace XLN.Game.Unity
{
    public class UnityAssetBundle<T> : Resource<T>
    {
        public UnityAssetBundle()
        {
        }

        public override T Deserialize(string key)
        {
            if(m_AssetBundle)
            {   if (key != null)
                {
                    //TODO: instaiate outside?
                    //GameObject go = (GameObject)UnityEngine.Object.Instantiate(m_AssetBundle.LoadAsset(key));
                    //return (T)(object)go;
                    return (T)(object)m_AssetBundle.LoadAsset(key);
                }
                else
                {
                    if(typeof(T) == typeof(UnityEngine.Object[]))
                        return (T)(object)m_AssetBundle.LoadAllAssets(typeof(T));
                }
            }   
            return default(T);
        }

        public override T Deserialize(Stream stream)
        {
            throw new NotImplementedException();
        }

        public override bool Load(ResourcePath path)
        {
            Stream stream = null;
            if (path.Type == ResourcePath.PathType.File)
            {
                stream = File.OpenRead(path.ResolveRealPath());

            }

            if (stream != null)
            {
                m_AssetBundle = AssetBundle.LoadFromStream(stream);
                stream.Close();
            }


            return true;
        }

        public override async Task<bool> LoadAsync(ResourcePath path)
        {
            Stream stream = null;
            if (path.Type == ResourcePath.PathType.File)
            {
                stream = File.OpenRead(path.ResolveRealPath());

            }

            if (stream != null)
            {
                m_AssetBundle = await AssetBundle.LoadFromStreamAsync(stream);
                stream.Close();
                return true;
            }
            return false;

            //return await AssetBundle.LoadFromStreamAsync()
        }

        protected override Stream GetStream()
        {
            throw new NotImplementedException();
        }

        private AssetBundle m_AssetBundle;
        private string[] m_AllAssetNames;

    }
}
