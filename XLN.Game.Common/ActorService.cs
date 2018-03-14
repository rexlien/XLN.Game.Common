//using System;
//using System.Runtime;
using Thrift.Protocol;
using Thrift.Transport;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
namespace XLN.Game.Common
{
    [GuidAttribute("FE12B484-4013-4AF2-8261-1C541F4A03C4")]
    public class ActorService : IService
    {

        public ActorService()
        {////
            SendEntityMessage();

            //IApplicationContext ctx = ContextRegistry.GetContext();   
        }

        public void SendEntityMessage()
        {////
            Task t = Task.Run(() =>
            {
                
                TFramedTransport transport = new TFramedTransport(new TSocket("10.56.148.186", 50000));
                //transport.Timeout = 3000;
                transport.Open();
                TProtocol protocol = new TBinaryProtocol(transport);
                //Thrift..Client client = new Thrift.EntityService.Client(protocol);

                //Thrift.EntityMessage msg = new Thrift.EntityMessage();
                //msg.Id = 0;
                //msg.Name = "MyEntity";
                //client.sendMessage(msg);
            });
        }
    }
}
