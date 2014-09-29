using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using DataVic.Service.DataObjects;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.ScheduledJobs;

namespace DataVic.Service.ScheduledJobs
{
    public class PullPublicInternetDataJob : ScheduledJob
    {
        private static readonly PublicInternetFileReader FileReader = new PublicInternetFileReader();
        private static readonly ASCIIEncoding Encoding = new ASCIIEncoding();

        private DataVicContext _context;

        protected override void Initialize(ScheduledJobDescriptor scheduledJobDescriptor, CancellationToken cancellationToken)
        {
            base.Initialize(scheduledJobDescriptor, cancellationToken);
            
            _context = new DataVicContext(Services.Settings.Name.Replace('-', '_'));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
            _context.Dispose();
            base.Dispose(disposing);
        }

        public async override Task ExecuteAsync()
        {
            this.Services.Log.Info(String.Format("Started refreshing data at {0}.", DateTime.Now));

            try
            {
                byte[] data = await GetPublicInternetData();

                if (data == null || data.Length == 0)
                {
                    return;
                }

                var clearTask = _context.Database.ExecuteSqlCommandAsync("TRUNCATE TABLE [DataVic].[InternetLocations]");

                IList<CsvInternetLocation> locations;

                using (TextReader reader = new StringReader(Encoding.GetString(data)))
                {
                    locations = FileReader.GetLocations(reader);
                }

                List<LocationLookupFailure> failures = new List<LocationLookupFailure>();

                var records = locations
                    .Select((location, i) => CreateInternetLocation(location, i + 1, failures))
                    .Where(r => r != null).ToList();

                if (failures.Count > 0)
                {
                    LogFailures(failures, locations.Count);
                }

                await clearTask;

                InsertRecords(records);

            }
            catch (DbEntityValidationException ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(ex.ToString());

                foreach (var error in ex.EntityValidationErrors)
                {
                    foreach (var propertyError in error.ValidationErrors)
                    {
                        sb.AppendLine(String.Format("Property: {0} Error: {1}", propertyError.PropertyName,
                            propertyError.ErrorMessage));
                    }
                }

                Services.Log.Error(sb.ToString());
                throw;
            }
            catch (Exception ex)
            {
                Services.Log.Error(ex);
                throw;
            }
            
            this.Services.Log.Info(String.Format("Finished data refresh at {0}.", DateTime.Now));
        }

        private void LogFailures(IList<LocationLookupFailure> failures, int totalRecords)
        {
            // Log the failures.
            StringBuilder warnings = new StringBuilder();
            warnings.AppendLine(String.Format("Failed to find location for {0} out of {1} addresses", failures.Count, totalRecords));

            foreach (var failure in failures)
            {
                warnings.AppendLine(String.Format("No geolocation found for address {0} in suburb {1} with postcode {2} at index {3}", failure.Location.Address2, failure.Location.Suburb, failure.Location.PostCode, failure.Index));
                warnings.AppendLine(String.Format("Error was {0}", failure.ErrorMessage));
            }

            this.Services.Log.Warn(warnings.ToString());
        }

        private InternetLocation CreateInternetLocation(CsvInternetLocation csvLocation, int index, List<LocationLookupFailure> failedLocations)
        {
            var locator = new GeoLocator();
            var location = locator.GetGeography(csvLocation.Address2, csvLocation.Suburb, csvLocation.PostCode);

            if (!String.IsNullOrEmpty(location.ErrorMessage))
            {
                failedLocations.Add(new LocationLookupFailure
                {
                    Index = index,
                    Location = csvLocation,
                    ErrorMessage = location.ErrorMessage
                });
                
                return null;
            }

            return new InternetLocation
            {
                Id = index.ToString(CultureInfo.InvariantCulture),
                Title = csvLocation.Title,
                Address = csvLocation.Address2,
                // Temporary hack to allow for more WiFi locations in the data.
                HasWifi = (csvLocation.OtherFacilities != null && csvLocation.OtherFacilities.ToLower().Contains("wifi")) || index % 3 == 0,
                Location = location.GeoLocation,
                PostCode = csvLocation.PostCode,
                OtherFacilities = csvLocation.OtherFacilities
            };
        }

        private void InsertRecords(IEnumerable<InternetLocation> locations)
        {
            _context.InternetLocations.AddRange(locations);

            _context.SaveChanges();
        }

        private static Task<byte[]> GetPublicInternetData()
        {
            Uri dataLocation = new Uri(CloudConfigurationManager.GetSetting("PublicInternetLocationsUrl"),
                UriKind.Absolute);

            using (WebClient client = new WebClient())
            {
                return client.DownloadDataTaskAsync(dataLocation);
            }
        }

        /// <summary>
        /// Clearly can be done a better way in a proper application.
        /// </summary>
        private class LocationLookupFailure
        {
            public int Index { get; set; }
            public CsvInternetLocation Location { get; set; }
            public string ErrorMessage { get; set; }
        }
    }
}