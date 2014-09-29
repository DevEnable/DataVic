using DataVic.Mobile;
using MonoTouch.CoreLocation;
using MonoTouch.MapKit;

namespace DataVic.iPhone
{
    public class InternetAnnotation : MKAnnotation
    {
        private readonly InternetLocation _location;
        private CLLocationCoordinate2D _coordinate;

        public override string Title
        {
            get
            {
                return _location.Title;
            }
        }

        public override CLLocationCoordinate2D Coordinate
        {
            get
            {
                return _coordinate;
            }
            set
            {
                _coordinate = value;
            }
        }

        public override string Subtitle
        {
            get
            {
                return _location.OtherFacilities;
            }
        }

        public bool HasWiFi
        {
            get
            {
				return _location.HasWifi;
            }
        }

        public InternetAnnotation(InternetLocation location)
        {
            _location = location;
            _coordinate = new CLLocationCoordinate2D(location.Location.Latitude, location.Location.Longitude);
        }

    }
}