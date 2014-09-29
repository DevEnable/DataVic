using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;

namespace DataVic.Service.ScheduledJobs
{
    public class PublicInternetFileReader
    {
        public IList<CsvInternetLocation> GetLocations(TextReader reader)
        {
            CsvReader csvReader = new CsvReader(reader, new CsvConfiguration
            {
                HasHeaderRecord = true,
                IgnoreHeaderWhiteSpace = true,
                IsHeaderCaseSensitive = false
            });

            return csvReader.GetRecords<CsvInternetLocation>().ToList();
        }
    }
}