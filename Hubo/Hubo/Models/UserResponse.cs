using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubo
{
    class UserResponse
    {
        [JsonProperty(PropertyName= "success")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName= "result")]
        public string Result { get; set; }

        [JsonProperty(PropertyName= "error")]
        public string error { get; set; }

        [JsonProperty(PropertyName= "unAuthorizedRequest")]
        public bool UnAuthorizedRequest { get; set; }

    }
}
