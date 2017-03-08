namespace Hubo
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class GoogleGeoLocationResponse
    {
        [JsonProperty(PropertyName = "location")]
        public LatLng Location { get; set; }
    }

    public class LatLng
    {
        [JsonProperty(PropertyName = "lat")]
        public double Latitude { get; set; }

        [JsonProperty(PropertyName = "lng")]
        public double Longitude { get; set; }
    }
}
