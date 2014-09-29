using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Windows.Devices.Geolocation;
using DataVic.Mobile;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Toolkit;

namespace DataVic.WindowsPhone
{
    public partial class MainPage
    {
        private readonly DataClient _client = new DataClient();
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly Geolocator _locator;
        private readonly MapLayer _wifiRangeLayer = new MapLayer();

        public MainPage()
        {
            InitializeComponent();

            this.DataContext = this;

            _locator = new Geolocator
            {
                DesiredAccuracy = PositionAccuracy.High,
                MovementThreshold = 20,
            };

            _locator.StatusChanged += HandleGeolocatorStatusChanged;
            _locator.PositionChanged += HandleGeoLocatorPositionChanged;

            this.InternetLocationsMap.Layers.Add(_wifiRangeLayer);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains("LocationConsent"))
            {
                // User has opted in or out of Location
                return;
            }

            MessageBoxResult result =
                    MessageBox.Show("This app accesses your phone's location. Is that ok?",
                    "Location",
                    MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = true;
            }
            else
            {
                IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = false;
            }

            IsolatedStorageSettings.ApplicationSettings.Save();
        }

        private void HandleGeolocatorStatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            string status = "";

            switch (args.Status)
            {
                case PositionStatus.Disabled:
                    // the application does not have the right capability or the location master switch is off
                    status = "location is disabled in phone settings";
                    break;
                case PositionStatus.Initializing:
                    // the geolocator started the tracking operation
                    status = "initializing";
                    break;
                case PositionStatus.NoData:
                    // the location service was not able to acquire the location
                    status = "no data";
                    break;
                case PositionStatus.Ready:
                    // the location service is generating geopositions as specified by the tracking parameters
                    status = "ready";
                    break;
                case PositionStatus.NotAvailable:
                    status = "not available";
                    // not used in WindowsPhone, Windows desktop uses this value to signal that there is no hardware capable to acquire location information
                    break;
                case PositionStatus.NotInitialized:
                    // the initial state of the geolocator, once the tracking operation is stopped by the user the geolocator moves back to this state

                    break;
            }

            Dispatcher.BeginInvoke(() =>
            {
                StatusText.Text = status;
            });
        }

        private void HandleGeoLocatorPositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            GetNearbyInternetLocations(args);
        }

        private async Task GetNearbyInternetLocations(PositionChangedEventArgs args)
        {

            try
            {
                var locationsTask = _client.GetLocations(args.Position.Coordinate.Latitude, args.Position.Coordinate.Longitude);
                Dispatcher.BeginInvoke(() => CentreOnLocation(args.Position.Coordinate));
                IEnumerable<InternetLocation> locations = await locationsTask;
                Dispatcher.BeginInvoke(() => MarkInternetLocations(locations.ToList()));
            }
            catch (Exception ex)
            {
                Dispatcher.BeginInvoke(() => MessageBox.Show(ex.ToString()));
            }
        }

        private void CentreOnLocation(Geocoordinate location)
        {
            this.InternetLocationsMap.Center = new GeoCoordinate(location.Latitude, location.Longitude);
            this.InternetLocationsMap.ZoomLevel = 15;
        }

        private void MarkInternetLocations(IList<InternetLocation> locations)
        {
            _wifiRangeLayer.Clear();

            var itemCollection = MapExtensions.GetChildren(this.InternetLocationsMap).OfType<MapItemsControl>().FirstOrDefault();

            if (itemCollection.Items.Count > 0)
            {
                itemCollection.Items.Clear();
            }
            
            BitmapImage range = new BitmapImage();
            range.SetSource(Application.GetResourceStream(new Uri(@"Assets/WiFiArea.png", UriKind.Relative)).Stream);

            foreach (var location in locations)
            {
                GeoCoordinate coordinate = new GeoCoordinate(location.Location.Latitude, location.Location.Longitude);

                itemCollection.Items.Add(new PushpinModel
                {
                    Details = location,
                    Location = coordinate
                });

                if (location.HasWifi)
                {
                    Ellipse circleRange = new Ellipse
                    {
                        Fill = new ImageBrush
                        {
                            ImageSource = range
                        },
                        Height = 64,
                        Width = 64
                    };

                    MapOverlay wifiRange = new MapOverlay
                    {
                        Content = circleRange,
                        GeoCoordinate = coordinate,
                        PositionOrigin = new Point(0.5, 0.5)
                    };

                    _wifiRangeLayer.Add(wifiRange);
                }
            }

            this.LocationCount.Visibility = Visibility.Visible;
            this.LocationCount.Text = String.Format("{0} nearby locations", locations.Count());
        }

    }
}