using Newtonsoft.Json;

namespace DataVic.Service.Models
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class MapLocation
    {
        [JsonProperty]
        public double Longitude { get; set; }

        [JsonProperty]
        public double Latitude { get; set; }
    }
}