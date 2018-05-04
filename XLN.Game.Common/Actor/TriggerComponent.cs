using System;
using System.Runtime.InteropServices;

namespace XLN.Game.Common.Actor
{
    [GuidAttribute("E87A4775-3482-40FD-89F0-918EEEBF1E65")]
    public class TriggerComponent : BaseComponent
    {
        private BaseActor m_Owner;
        public BaseActor Owner
        {
            get
            {
                return m_Owner;
            }

            set
            {
                m_Owner = value;
            }
        }
    }
}
