using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using XLN.Game.Common;
using XLN.Game.Unity.Extension;

namespace XLN.Game.Unity
{
    public class UnityAssetBundle : Resource<AssetBundle>
    {

        class AssetBundleConverter : Converter<UnityEngine.GameObject>
        {
            public AssetBundleConverter()
            {
                
            }
            public override UnityEngine.GameObject Convert()
            {
               
                return base.Convert();
            }

            private AssetBundle m_AssetBundle;
        }

        public UnityAssetBundle()
        {

            //m_ConverterMap.Add(typeof(UnityEngine.GameObject), new AssetBundleConverter());
        }

        public override R Deserialize<R>(string key)
        {
            if(m_Resource)
            {   if (key != null)
                {
                    //TODO: instaiate outside?
                    //GameObject go = (GameObject)UnityEngine.Object.Instantiate(m_AssetBundle.LoadAsset(key));
                    //return (T)(object)go;
                    return (R)(object)m_Resource.LoadAsset(key);
                }
                else
                {
                    if(typeof(R) == typeof(UnityEngine.Object[]))
                        return (R)(object)m_Resource.LoadAllAssets(typeof(R));
                }
            }   
            return default(R);
        }

        public override bool Load(ResourcePath path)
        {
            if (path.Type == ResourcePath.PathType.File)
            {
                Stream stream = null;
                stream = File.OpenRead(path.ResolveRealPath());
                if (stream != null)
                {
                    m_Resource = AssetBundle.LoadFromStream(stream);
                    stream.Close();
                    return true;
                }

            }
            else if (path.Type == ResourcePath.PathType.HTTP)
            {
                UnityWebRequest www = UnityWebRequest.GetAssetBundle(path.ResolveRealPath());
                www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    return false;
                }
                m_Resource = DownloadHandlerAssetBundle.GetContent(www);
            }
            return false;
        }

        public override async Task<bool> LoadAsync(ResourcePath path)
        {
            
            if (path.Type == ResourcePath.PathType.File)
            {
                Stream stream = null;
                stream = File.OpenRead(path.ResolveRealPath());
                if (stream != null)
                {
                    m_Resource = await AssetBundle.LoadFromStreamAsync(stream);
                    stream.Close();
                    return true;
                }

            }
            else if(path.Type == ResourcePath.PathType.HTTP)
            {
                UnityWebRequest www = UnityWebRequest.GetAssetBundle(path.ResolveRealPath());
                await www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    return false;
                }
                m_Resource = DownloadHandlerAssetBundle.GetContent(www);
            }
            return false;

        }


        //private AssetBundle m_AssetBundle;
        private string[] m_AllAssetNames;

    }
}
