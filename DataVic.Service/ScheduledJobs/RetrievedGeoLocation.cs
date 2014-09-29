using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;

namespace DataVic.Service.ScheduledJobs
{
    public class RetrievedGeoLocation
    {
        public DbGeography GeoLocation { get; set; }

        public string ErrorMessage { get; set; }
    }
}