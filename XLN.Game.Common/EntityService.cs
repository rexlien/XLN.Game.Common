using System;
using System.Runtime;
using Thrift.Protocol;
using Thrift.Transport;

namespace XLN.Game.Common
{
    public class EntityService
    {

        public EntityService()
        {
            SendEntityMessage();
        }

        public void SendEntityMessage()
        {
            TTransport transport = new TSocket("localhost", 10000);
            TProtocol protocol = new TCompactProtocol(transport);
            Thrift.EntityService.Client client = new Thrift.EntityService.Client(protocol);
        }
    }
}
