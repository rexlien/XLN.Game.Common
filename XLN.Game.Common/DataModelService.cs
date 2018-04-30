using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using XLN.Game.Common.Actor;
using XLN.Game.Common.DataModel;

namespace XLN.Game.Common
{
    [GuidAttribute("168AE378-10CD-4899-A1C5-6C0F9DC2BAEA")]
    public class DataModelService : IService
    {
        public DataModelService()
        {
            
        }

        public override bool OnInit()
        {
            m_ResourceService = ServiceMgr.GetServiceMgr().GetService<ResourceService>();
            return base.OnInit();
        }

        public void AddAttributeCollection(string collectionName, string filePath)
        {
            AttributeCollection attributes = m_ResourceService.Get<AttributeCollection>(new ResourcePath(ResourcePath.PathType.Resource, filePath)).Result;
            if(attributes != null)
            {
                m_AttributeCollections.Add(collectionName, attributes);
            }
        }

        public void RegisterAttributeComponent<T>(string filePath)// where T : AttributeComponent
        {
            AddAttributeCollection(typeof(T).FullName, filePath);
        }


        private Dictionary<string, AttributeCollection> m_AttributeCollections = new Dictionary<string, AttributeCollection>();
        private ResourceService m_ResourceService;

        public Dictionary<string, AttributeCollection> AttributeCollections { get => m_AttributeCollections; }
    }
}
