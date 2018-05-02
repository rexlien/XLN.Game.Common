using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace XLN.Game.Common.Actor
{
    [GuidAttribute("0E0247F8-EECD-4CD5-ABFE-5F82309BBA6B")]
    public class TransformComponent : BaseComponent
    {
        public TransformComponent()
        {
        }

        public TransformComponent(Vector3 pos, Quaternion rotation, Vector3? scale = null)
        {
            m_Position = pos;
            m_Rotation = rotation;
            if(scale.HasValue)
            {
                m_Scale = scale.Value;
            }
        }


        private Vector3 m_Position = Vector3.Zero;
        private Quaternion m_Rotation = Quaternion.Identity;
        private Vector3 m_Scale = Vector3.One;

        public Vector3 Position { get => m_Position; set => m_Position = value; }
        public Quaternion Rotation { get => m_Rotation; set => m_Rotation = value; }
        public Vector3 Scale { get => m_Scale; set => m_Scale = value; }
    }
}
