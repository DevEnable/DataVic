using Newtonsoft.Json;

namespace DataVic.Mobile
{
    public class MapLocation
    {
        [JsonProperty("longitude")]
        public double Longitude { get; set; }

        [JsonProperty("latitude")]
        public double Latitude { get; set; }
    }
}
