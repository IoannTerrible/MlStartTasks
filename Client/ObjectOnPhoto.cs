using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace SocketClient
{
    public class ObjectOnPhoto
    {
        [JsonProperty("xtl")]
        public double XTopLeftCorner { get; set; }

        [JsonProperty("ytl")]
        public double YTopLeftCorner { get; set; }

        [JsonProperty("xbr")]
        public double XBottonRigtCorner { get; set; }

        [JsonProperty("ybr")]
        public double YBottonRigtCorner { get; set; }

        [JsonProperty("class_name")]
        public string Class_name { get; set; }
    }
}
