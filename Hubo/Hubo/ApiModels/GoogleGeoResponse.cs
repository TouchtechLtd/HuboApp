using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubo
{
    public class GoogleGeoResponse
    {
        [JsonProperty(PropertyName = "results")]
        public List<AddressResult> Results { get; set; }
    }

    public class AddressResult
    {
        [JsonProperty(PropertyName = "formatted_address")]
        public string FormattedAddress { get; set; }
    }
}
