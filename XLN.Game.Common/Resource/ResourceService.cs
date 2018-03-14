using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace XLN.Game.Common
{
    
    public class ResourceService : IService
    {


        public void SetResourcePath(string path)
        {
            
        }

        virtual protected Task<Resource<T>> GetResourceAsync<T>(ResourcePath path)
        {
            return null;
        }

        virtual protected Resource<T> GetResource<T>(ResourcePath path)
        {
            return null;
        }

        virtual public Task<T> Get<T>(ResourcePath path)
        {
            return Task.FromResult(default(T));
        }

        virtual public async Task<T> GetAsync<T>(ResourcePath path)
        {
            return await Task.Run(() => default(T));
           
        }
        /*
        public virtual string ResolveRealPath(ResourcePath path)
        {
            return path.Path;
        }
*/
        public virtual void RegisterResourceCreator<TargetResource>(string ext)
        {
            
        }


        protected ConcurrentDictionary<string, ICacheable> m_Resources = new ConcurrentDictionary<string, ICacheable>();

    }

}