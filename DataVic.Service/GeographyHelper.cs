using System.Data.Entity.Spatial;

namespace DataVic.Service
{
    public static class GeographyHelper
    {
        public static DbGeography FromLatitudeLongitude(double latitude, double longitude)
        {
            return DbGeography.PointFromText(string.Format("POINT({0} {1})", longitude, latitude), 4326);
        }
    }
}