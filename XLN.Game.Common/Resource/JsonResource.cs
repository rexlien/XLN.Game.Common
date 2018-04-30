using System;
using System.IO;
using Newtonsoft.Json;

namespace XLN.Game.Common.Resource
{
    public class JsonResource : Resource<string>
    {
        public JsonResource()
        {
        }

        public override R Deserialize<R>(string key)
        {
            R res = JsonConvert.DeserializeObject<R>(m_Resource);
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
