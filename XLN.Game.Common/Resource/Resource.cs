using System;
using System.IO;
using System.Threading.Tasks;

namespace XLN.Game.Common
{
    public abstract class Resource<T> : ICacheable, IStreamable<T>
    {
        
        public abstract T Deserialize(string key);
        public abstract T Deserialize(Stream stream);
        public abstract bool Load(ResourcePath path);
        public virtual async Task<bool> LoadAsync(ResourcePath path)
        {
            return await Task.Run(() => { return Task.FromResult(false);});
        }
        protected abstract Stream GetStream();

        //protected ResourcePath m_ResourcePath;
    }
}
