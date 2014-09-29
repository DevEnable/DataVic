using System.Data.Entity.Spatial;
using Microsoft.WindowsAzure.Mobile.Service;

namespace DataVic.Service.DataObjects
{
    public class InternetLocation : EntityData
    {
        public string Title { get; set; }

        public DbGeography Location { get; set; }

        public string Address { get; set; }

        public int PostCode { get; set; }

        public bool HasWifi { get; set; }

        public string OtherFacilities { get; set; }
    }
}