using Newtonsoft.Json;

namespace DataVic.Mobile
{
    public class InternetLocation
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("location")]
        public MapLocation Location { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("haswifi")]
        public bool HasWifi { get; set; }

        [JsonProperty("otherfacilities")]
        public string OtherFacilities { get; set; }

    }
}
