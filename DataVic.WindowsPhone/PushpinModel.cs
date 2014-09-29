using System.Device.Location;
using DataVic.Mobile;

namespace DataVic.WindowsPhone
{
    public class PushpinModel
    {
        public InternetLocation Details { get; set; }
        public GeoCoordinate Location { get; set; }
    }
}
