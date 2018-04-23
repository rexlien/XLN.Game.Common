using System;
using System.Threading.Tasks;
using UnityEngine;
using XLN.Game.Common;
using XLN.Game.Common.Config;

namespace XLN.Game.Unity
{
    public class ServiceBehavior : MonoBehaviour
    {
        void Awake()
        {
            ServiceMgr.GetServiceMgr().RegisterService(new UnityLogService());
            ServiceMgr.GetServiceMgr().RegisterService(new GameObjectPoolService());
            //LogService service =ServiceMgr.GetServiceMgr().GetService<LogService>();
            XLN.Game.Common.ApplicationContext.Init(new UnityResourceService(), UnitySystem.UnityScheduler);
        }


        void Start()
        {



        }

        void Update()
        {
            ServiceMgr.GetServiceMgr().Update(Time.deltaTime);
        }

        void OnApplicationQuit()
        {
            ServiceMgr.GetServiceMgr().OnDestroy();
        }

    }
}
