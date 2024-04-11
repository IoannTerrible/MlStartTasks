using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class ResponseObject
    {
        [JsonProperty("objects")]
        public ObjectOnPhoto[] Objects { get; set; }
    }

}
