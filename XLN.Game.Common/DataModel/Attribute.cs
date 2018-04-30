using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using XLN.Game.Common.Actor;

namespace XLN.Game.Common.DataModel
{
    public class Attribute// : BaseComponent
    {
        public Attribute()
        {

        }

        //[JsonProperty]
        //public int ID;

        [JsonProperty]
        public Dictionary<string, object> Attributes = new Dictionary<string, object>();
    }
}
