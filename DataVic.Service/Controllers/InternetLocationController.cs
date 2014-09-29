using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using AutoMapper;
using DataVic.Service.DataObjects;
using DataVic.Service.Models;
using Microsoft.WindowsAzure.Mobile.Service;

namespace DataVic.Service.Controllers
{
    public class InternetLocationController : TableController<InternetLocationModel>
    {
        private DataVicContext _context;

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            _context = new DataVicContext(Services.Settings.Name);
            this.DomainManager = new InternetLocationDomainManager(_context, Request, Services);
        }

        /// <summary>
        /// Gets all of the locations that have internet available.
        /// </summary>
        /// <returns>Internet locations.</returns>
        public IQueryable<InternetLocationModel> GetLocations()
        {
            return this.Query();
        }

        /// <summary>
        /// Gets all of the internet locations given near a given geography point and within a certain range.
        /// </summary>
        /// <param name="longitude">Longitude of the location to use as the center of the search.</param>
        /// <param name="latitude">Latitude of the location to use as the center of the search.</param>
        /// <param name="distance">Distance in metres to search out from.</param>
        /// <returns>All of the internet locations within the distance from the given point.</returns>
        public IEnumerable<InternetLocationModel> GetLocationsNear(double latitude, double longitude, long distance = 2000)
        {
            try
            {
                // http://rbrundritt.wordpress.com/2012/06/08/entity-framework-5-bing-maps-wpf/ Used as the baseline for this.
                DbGeography geoLocation = GeographyHelper.FromLatitudeLongitude(latitude, longitude);
                var dbLocations = (from l in _context.InternetLocations
                                   where l.Location.Distance(geoLocation) < distance
                                   select l).ToList();

                return dbLocations.Select(Mapper.Map<InternetLocationModel>);
            }
            catch (Exception ex)
            {
                Services.Log.Error(ex);
                throw;
            }
        }
    }
}