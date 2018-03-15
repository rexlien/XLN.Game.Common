using System;
using System.IO;
using System.Xml.Serialization;

namespace XLN.Game.Common
{
    public class XMLResource : Resource<string>
    {
        public XMLResource()
        {
        }
       
        public override R Deserialize<R>(string key)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(R));
            R res = (R)serializer.Deserialize(GetStream());
            return res;
        }

        public override bool Load(ResourcePath path)
        {
            return false;
        }

        protected virtual Stream GetStream()
        {
            return null;
        }


       

    }
}
