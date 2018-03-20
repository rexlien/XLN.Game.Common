using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace XLN.Game.Common.Config
{
    
    [XmlRootAttribute("XLNApp", Namespace = "", IsNullable = false)]
    public class AppConfig
    {
        public AppConfig()
        {

        }

        public class Service
        {

            [System.Xml.Serialization.XmlAttribute("name")]
            public string Name { get; set; }

            [System.Xml.Serialization.XmlAttribute("class")]
            public string Class { get; set; }

            [System.Xml.Serialization.XmlAttribute("assemblyName")]
            public string AssemblyName { get; set; }


        }

        public class Server
        {

            [System.Xml.Serialization.XmlAttribute("name")]
            public string Name { get; set; }

            [System.Xml.Serialization.XmlAttribute("ip")]
            public string IP { get; set; }

            [System.Xml.Serialization.XmlAttribute("port")]
            public int Port { get; set; }


        }


        public class Services
        {
            [XmlElement("Service")]
            public List<Service> ServiceItems { get; set; }
        }

        public class Networks
        {
            [XmlElement("Server")]
            public List<Server> ServerItems  { get; set; }
        }


        [XmlElement("Services")]
        public Services AppServices { get; set; }

        [XmlElement("Network")]
        public Networks AppNetworks { get; set; }

    }

}


