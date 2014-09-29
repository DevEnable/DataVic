using System.Data.Entity;
using System.Data.Entity.Spatial;
using System.Web.Http;
using AutoMapper;
using DataVic.Service.DataObjects;
using DataVic.Service.Models;
using Microsoft.WindowsAzure.Mobile.Service;

namespace DataVic.Service
{
    public static class WebApiConfig
    {
        public static void Register()
        {
            // Use this class to set configuration options for your mobile service
            ConfigOptions options = new ConfigOptions();

            // Use this class to set WebAPI configuration options
            HttpConfiguration config = ServiceConfig.Initialize(new ConfigBuilder(options));

            // To display errors in the browser during developemnt, uncomment the following
            // line. Comment it out again when you deploy your service for production use.
            // config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            
            Database.SetInitializer(new DataVicInitializer());

            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<DbGeography, MapLocation>()
                    .ForMember(dst => dst.Latitude, map => map.MapFrom(s => s.Latitude.Value))
                    .ForMember(dst => dst.Longitude, map => map.MapFrom(s => s.Longitude.Value));
                    
                cfg.CreateMap<InternetLocation, InternetLocationModel>()
                    .ForMember(dst => dst.Location, map => map.MapFrom(s => s.Location));
            });
        }
    }

    public class DataVicInitializer : DropCreateDatabaseIfModelChanges<DataVicContext>
    {
        
    }
}

