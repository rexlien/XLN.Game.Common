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

        void Start()
        {
            ServiceMgr.GetServiceMgr().Init();
        }

        void Update()
        {
            ServiceMgr.GetServiceMgr().Update(Time.deltaTime);    
        }

    }
}
