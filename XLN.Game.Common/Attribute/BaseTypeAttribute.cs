using System;
namespace XLN.Game.Common
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited=true)]
    public class BaseTypeAttribute : Attribute
    {
        public BaseTypeAttribute(Type type, string baseComponentName = null)
        {
            m_Type = type;
            m_BaseComponentName = baseComponentName;
        }

       

        string m_BaseComponentName;
        Type m_Type;

        public string BaseComponentName { get => m_BaseComponentName; set => m_BaseComponentName = value; }
        public Type Type { get => m_Type; set => m_Type = value; }
    }
}
