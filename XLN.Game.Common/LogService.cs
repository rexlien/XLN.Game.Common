using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace XLN.Game.Common
{
    
    public abstract class LogService : IService
    {

        public enum LogType
        {
            LT_DEBUG,
            LT_WARNING,
            LT_ERROR
        };

        private static LogService s_Log;
        public static LogService Logger
        {
            private set { }
            get
            {
                if (s_Log == null)
                {
                    
                    s_Log = ServiceMgr.GetServiceMgr().GetService<LogService>();
                    
                }
                return s_Log;
            }

        }

        public abstract void Log(LogType type, string log);
    }
}

