namespace Hubo
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class GoogleGeoResponse
    {
        [JsonProperty(PropertyName = "results")]
        public List<AddressResult> Results { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }

    public class AddressResult
    {
        [JsonProperty(PropertyName = "address_components")]
        public List<GoogleAddressComponents> GoogleAddressComponents { get; set; }
    }

    public class GoogleAddressComponents
    {
        [JsonProperty(PropertyName = "long_name")]
        public string LongName { get; set; }

        [JsonProperty(PropertyName = "short_name")]
        public string ShortName { get; set; }
    }
}
