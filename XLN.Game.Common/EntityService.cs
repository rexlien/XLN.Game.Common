//using System;
//using System.Runtime;
using Thrift.Protocol;
using Thrift.Transport;
using System.Threading.Tasks;

namespace XLN.Game.Common
{
    public class EntityService
    {

        public EntityService()
        {////
            SendEntityMessage();
        }

        public void SendEntityMessage()
        {////
            Task t = Task.Run(() =>
            {
                
                TFramedTransport transport = new TFramedTransport(new TSocket("10.56.148.186", 50000));
                //transport.Timeout = 3000;
                transport.Open();
                TProtocol protocol = new TBinaryProtocol(transport);
                Thrift.EntityService.Client client = new Thrift.EntityService.Client(protocol);

                Thrift.EntityMessage msg = new Thrift.EntityMessage();
                msg.Id = 0;
                msg.Name = "MyEntity";
                client.sendMessage(msg);
            });
        }
    }
}
