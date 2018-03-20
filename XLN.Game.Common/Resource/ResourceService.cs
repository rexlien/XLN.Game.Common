using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace XLN.Game.Common
{

    public class ResourceService : IService
    {

        public class IResourceCreator
        {
            public virtual IResource Create()
            {
                return null;
            }
        }

        public class ResoruceCreator<T> : IResourceCreator where T : IResource
        {
            public override IResource Create()
            {
                return Activator.CreateInstance<T>();
            }
        }

        public void SetResourcePath(string path)
        {
            
        }

        virtual protected Task<IResource> GetResourceAsync(ResourcePath path)
        {
            return null;
        }

        virtual protected IResource GetResource(ResourcePath path)
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

        virtual public Task<T> GetAsync<T>(ResourcePath path, Func<T, bool> callback)
        {
            callback(default(T));
            return Task.FromResult(default(T));
        }
        /*
        public virtual string ResolveRealPath(ResourcePath path)
        {
            return path.Path;
        }
*/
        public virtual void RegisterResourceCreator(string ext, IResourceCreator resCreator)// where T : IResource
        {
            m_ResoruceCreators.Add(ext, resCreator);
        }

        public virtual IResourceCreator GetCreator(string ext)
        {
            IResourceCreator creator;
            m_ResoruceCreators.TryGetValue(ext, out creator);
            return creator;
        }


        protected ConcurrentDictionary<string, ICacheable> m_Resources = new ConcurrentDictionary<string, ICacheable>();
        protected Dictionary<string, IResourceCreator > m_ResoruceCreators = new Dictionary<string, IResourceCreator>();

    }

}