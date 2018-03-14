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


        public class Services
        {
            [XmlElement("Service")]
            public List<Service> ServiceItems { get; set; }
        }




        [XmlElement("Services")]
        public Services AppServices { get; set; }

    }

}


