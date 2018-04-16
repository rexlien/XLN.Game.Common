using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace XLN.Game.Common
{
    public abstract class IService
    {

        public virtual bool OnInit()
        {
            return true;
        }
        public virtual bool OnDestroy()
        {
            return true;
        }
        public virtual bool OnUpdate(float delta)
        {
            return true;
        }
        public virtual bool OnPostUpdate(float delta)
        {
            return true;
        }
        public virtual void OnEvent(int eventID)
        {
            
        }
    }
}
