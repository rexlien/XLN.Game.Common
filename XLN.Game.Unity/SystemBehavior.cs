using System;
using System.Threading.Tasks;
using UnityEngine;
using XLN.Game.Common;
using XLN.Game.Common.Config;


namespace XLN.Game.Unity
{
    public class SystemBehavior :  MonoBehaviour
    {
        void Awake()
        {
            ServiceMgr.GetServiceMgr().RegisterService(new UnityLogService());

            UnityResourceService resourceService = new UnityResourceService();
            ServiceMgr.GetServiceMgr().RegisterService(resourceService);
            Task<AppConfig> appConfig = resourceService.Get<AppConfig>(new ResourcePath(ResourcePath.PathType.Resource, "XLNConfig.xml"));
            if (appConfig != null)
            {
                ServiceMgr.GetServiceMgr().InjectService(appConfig.Result);
            }
           
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
