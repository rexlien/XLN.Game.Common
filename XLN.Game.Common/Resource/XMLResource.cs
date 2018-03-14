using System;
using System.IO;
using System.Xml.Serialization;

namespace XLN.Game.Common
{
    public class XMLResource<T> : Resource<T>
    {
        public XMLResource()
        {
        }

        public override T Deserialize(Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            T res = (T)serializer.Deserialize(stream);
            return res;
        }

        public override T Deserialize(string key)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            T res = (T)serializer.Deserialize(GetStream());
            return res;
        }

        public override bool Load(ResourcePath path)
        {
            return false;
        }

        protected override Stream GetStream()
        {
            return null;
        }


       

    }
}
