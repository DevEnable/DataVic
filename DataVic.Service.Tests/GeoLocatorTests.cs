using System.Data.Entity.Spatial;
using DataVic.Service.ScheduledJobs;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataVic.Service.Tests
{
    /// <summary>
    /// Integration tests for the <see cref="GeoLocator"/> class.
    /// </summary>
    [TestClass]
    public class GeoLocatorTests
    {
        [TestMethod]
        public void GetGeography_FindsAddress()
        {
            // Arrange
            GeoLocator locator = new GeoLocator();

            // Act
            DbGeography geography = locator.GetGeography("7 High Street", "Hastings", 3915).GeoLocation;

            // Assert
            geography.Should().NotBeNull();
            geography.Latitude.Should().NotBe(0);
            geography.Longitude.Should().NotBe(0);
        }

        [TestMethod]
        public void GetGeography_DifferentAddresses_DifferentLatLong()
        {
            // Arrange
            GeoLocator locator = new GeoLocator();
            GeoLocator otherLocator = new GeoLocator();

            // Act
            DbGeography geography = locator.GetGeography("7 High Street", "Hastings", 3915).GeoLocation;
            DbGeography other = otherLocator.GetGeography("1255 High Street", "Malvern", 3144).GeoLocation;
            
            // Assert
            geography.Should().NotBeNull();
            geography.Latitude.Should().NotBe(other.Latitude);
            geography.Longitude.Should().NotBe(other.Longitude);
        }

        [TestMethod]
        public void GetGeography_CanFindFlindersLane()
        {
            // Arrange
            GeoLocator locator = new GeoLocator();

            // Act
            DbGeography geography = locator.GetGeography("247 Flinders Lane", "Melbourne", 3000).GeoLocation;

            // Assert
            geography.Should().NotBeNull();
            geography.Latitude.Should().NotBe(0);
            geography.Longitude.Should().NotBe(0);
        }
    }
}
