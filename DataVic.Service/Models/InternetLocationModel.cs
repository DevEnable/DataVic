using System;
using Microsoft.WindowsAzure.Mobile.Service.Tables;
using Newtonsoft.Json;

namespace DataVic.Service.Models
{
    [JsonObject(Title = "InternetLocation", MemberSerialization = MemberSerialization.OptIn)]
    public class InternetLocationModel : ITableData
    {
        public string Id { get; set; }

        public byte[] Version { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public bool Deleted { get; set; }

        [JsonProperty]
        public string Title { get; set; }

        [JsonProperty]
        public MapLocation Location { get; set; }

        [JsonProperty]
        public string Address { get; set; }

        [JsonProperty]
        public bool HasWifi { get; set; }

        [JsonProperty]
        public string OtherFacilities { get; set; }
    }
}