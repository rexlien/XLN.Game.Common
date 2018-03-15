using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace XLN.Game.Common
{
    public abstract class Resource<T> : IResource
    {

        public abstract R Deserialize<R>(string key);
        public abstract bool Load(ResourcePath path);
        public virtual async Task<bool> LoadAsync(ResourcePath path)
        {
            return await Task.Run(() => { return Task.FromResult(false);});
        }
        public virtual Converter<R> GetConverter<R>()
        {
            IConverter iconverter;
            m_ConverterMap.TryGetValue(typeof(R), out iconverter);
            return (Converter<R>)(iconverter);
        }

        protected T m_Resource;
        protected Dictionary<Type, XLN.Game.Common.IConverter> m_ConverterMap = new Dictionary<Type, XLN.Game.Common.IConverter>();

    }
}
