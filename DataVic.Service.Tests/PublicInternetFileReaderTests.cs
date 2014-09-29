using System.IO;
using System.Linq;
using System.Reflection;
using DataVic.Service.ScheduledJobs;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataVic.Service.Tests
{
    /// <summary>
    /// Integration tests for the <see cref="PublicInternetFileReader"/> class.
    /// </summary>
    [TestClass]
    public class PublicInternetFileReaderTests
    {
        [TestMethod]
        public void CsvFileReader_GetLocations_ReturnsAllLocations()
        {
            // Arrange
            StreamReader stream = CreateStreamReader();
            PublicInternetFileReader reader = new PublicInternetFileReader();

            // Act
            var records = reader.GetLocations(stream);

            // Assert
            records.Count.Should().Be(686);
            records.All(csv => csv.Address2 != null && csv.PostCode > 0).Should().BeTrue();
        }

        private StreamReader CreateStreamReader()
        {
            var data = Assembly.GetExecutingAssembly().GetManifestResourceStream("DataVic.Service.Tests.publicweb.csv");
            return new StreamReader(data);
        }
    }
}
