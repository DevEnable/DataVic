using System;
using BingGeocoder;

namespace DataVic.Service.ScheduledJobs
{
    public class GeoLocator
    {
        private readonly BingGeocoderClient _client = new BingGeocoderClient("yourbingcodehere");
        
        public RetrievedGeoLocation GetGeography(string address, string suburb, int postCode)
        {
            BingGeocoderResult result = _client.Geocode(String.Format("{0}, {1}, {2}, Australia", address, suburb, postCode));

            if (!String.IsNullOrEmpty(result.ErrorMessage))
            {
                return new RetrievedGeoLocation
                {
                    ErrorMessage = result.ErrorMessage
                };
            }

            if (result.Confidence == "No results")
            {
                return new RetrievedGeoLocation
                {
                    ErrorMessage = "No results"
                };
            }

            return new RetrievedGeoLocation
            {
                GeoLocation = GeographyHelper.FromLatitudeLongitude(double.Parse(result.Latitude), double.Parse(result.Longitude))
            };
        }
    }
}