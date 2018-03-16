using System;
namespace XLN.Game.Common
{
    public class NetworkService : IService
    {
        public NetworkService()
        {
        }


        XLN.Game.Common.Thrift.NetworkService.Client m_Client;
    }
}
