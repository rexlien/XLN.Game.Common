using System;
using System.IO;
using System.Text;
using UnityEngine;
using XLN.Game.Common;
using XLN.Game.Unity.Extension;

namespace XLN.Game.Unity
{
    public class UnityXMLResource : XMLResource
    {
        public UnityXMLResource()
        {
            
        }

        protected override Stream GetStream()
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(m_Resource);
            MemoryStream stream = new MemoryStream(byteArray);
            return stream;
        }

        public override bool Load(ResourcePath path)
        {
            TextAsset asset = (TextAsset)Resources.Load(path.ResolveRealPath());
            if (!asset)
                return false;
            m_Resource = String.Copy(asset.text);
            return true;
        }

        //private string m_XMLText;

    };
}
