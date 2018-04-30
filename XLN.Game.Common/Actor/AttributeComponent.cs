using System;
using XLN.Game.Common.DataModel;

namespace XLN.Game.Common.Actor
{
    public class AttributeComponent : BaseComponent
    {
        public AttributeComponent(string key)
        {
            DataModelService = ServiceMgr.GetServiceMgr().GetService<DataModelService>();
            m_key = key;

            m_Attribute = DataModelService.AttributeCollections[this.GetType().FullName].DataAttributes[Key];
        }

        private XLN.Game.Common.DataModel.Attribute m_Attribute;
        private string m_key;

        public DataModelService DataModelService;

        public XLN.Game.Common.DataModel.Attribute Attribute { get => m_Attribute; }
        public string Key { get => m_key; }
    }
}
