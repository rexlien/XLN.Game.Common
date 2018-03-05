using System;
using UnityEngine;
using XLN.Game.Common;

namespace XLN.Game.Unity
{
    public class SystemBehavior :  MonoBehaviour
    {
        void Awake()
        {
            ServiceMgr.GetServiceMgr().RegisterService(new UnityLogService());
        }
    }
}
