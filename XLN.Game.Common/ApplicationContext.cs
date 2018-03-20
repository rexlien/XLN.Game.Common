using System;
using System.Threading.Tasks;
using XLN.Game.Common.Config;

namespace XLN.Game.Common
{
    public class ApplicationContext
    {
        public ApplicationContext()
        {
        }

        public static void Init(ResourceService resService, TaskScheduler mainScheduler)
        {

            ServiceMgr.GetServiceMgr().RegisterIService(resService);
            s_MainScheduler = mainScheduler;

            Task<AppConfig> appConfig = resService.Get<AppConfig>(new ResourcePath(ResourcePath.PathType.Resource, "XLNConfig.xml"));
            AppConig = appConfig.Result;
            if (AppConig != null)
            {
                ServiceMgr.GetServiceMgr().InjectService(AppConig);
            }
           
            ServiceMgr.GetServiceMgr().Init();
        }

        private static AppConfig s_AppConig;
        private static TaskScheduler s_MainScheduler;

        public static TaskScheduler MainScheduler { get => s_MainScheduler; set => s_MainScheduler = value; }
        public static AppConfig AppConig { get => s_AppConig; set => s_AppConig = value; }
    }
}
