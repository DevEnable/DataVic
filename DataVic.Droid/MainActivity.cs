using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Gms.Common;
using Android.Gms.Location;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Locations;
using Android.Util;
using Android.OS;
using DataVic.Mobile;

namespace DataVic.Droid
{
    [Activity(Label = "Free WiFi whereVic", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, Android.Gms.Location.ILocationListener, IGooglePlayServicesClientConnectionCallbacks, IGooglePlayServicesClientOnConnectionFailedListener
    {
        private readonly DataClient _client = new DataClient();
        private LocationClient _locationClient;
        private GoogleMap _map;
        private MapFragment _mapFragment;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            _locationClient = new LocationClient(this, this, this);

            SetContentView(Resource.Layout.Main);
            StartLocator();
            InitMapFragment();
            SetupMapIfNeeded();
        }

        protected override void OnResume()
        {
            base.OnResume();

            if (_locationClient.IsConnected)
            {
                RegisterForLocationUpdates();
            }
            else
            {
                StartLocator();
            }

            if (SetupMapIfNeeded())
            {
                _map.MyLocationEnabled = true;
            }
        }

        protected override void OnPause()
        {
            base.OnPause();

            if (_locationClient.IsConnected)
            {
                _locationClient.RemoveLocationUpdates(this);
            }

            _map.MyLocationEnabled = false;
        }

        public void OnConnected(Bundle bundle)
        {
            RegisterForLocationUpdates();
        }

        public void OnDisconnected()
        {
        }

        public void OnConnectionFailed(ConnectionResult p0)
        {
        }

        public async void OnLocationChanged(Location location)
        {
			try
            {
                var locationsTask = _client.GetLocations(location.Latitude, location.Longitude);
                CentreOnLocation(location);
                IEnumerable<InternetLocation> locations = await locationsTask;
                MarkInternetLocations(locations.ToList());
			}
			catch(Exception ex)
			{
			    Log.Error("Internet Location Search", ex.ToString());
			    throw;
			}
        }

        private void MarkInternetLocations(IEnumerable<InternetLocation> locations)
        {
            Bitmap wifiIcon = BitmapFactory.DecodeResource(Resources, Resource.Drawable.WiFi);
            wifiIcon = Bitmap.CreateScaledBitmap(wifiIcon, 64, 64, false);
            BitmapDescriptor wifiDescriptor = BitmapDescriptorFactory.FromBitmap(wifiIcon);

            Bitmap wifiRange = BitmapFactory.DecodeResource(Resources, Resource.Drawable.WiFiArea);
            BitmapDescriptor wifiRangeDescriptor = BitmapDescriptorFactory.FromBitmap(wifiRange);

            foreach (InternetLocation location in locations)
            {
                LatLng position = new LatLng(location.Location.Latitude, location.Location.Longitude);
                MarkerOptions marker = new MarkerOptions();
                marker.SetPosition(position);
                marker.SetTitle(location.Title);
                marker.SetSnippet(location.Address + System.Environment.NewLine + System.Environment.NewLine + location.OtherFacilities);
                
                if (location.HasWifi)
                {
                    marker.InvokeIcon(wifiDescriptor);
                    GroundOverlayOptions overlay = new GroundOverlayOptions()
                        .Position(position, 200, 200)
                        .InvokeImage(wifiRangeDescriptor);
                    _map.AddGroundOverlay(overlay);
                }
                
                _map.AddMarker(marker);
            }
        }

        private void InitMapFragment()
        {
            // May not need the class definition on the map frame because of this.  Probably still want this though to configure the map options.
            _mapFragment = FragmentManager.FindFragmentByTag("internetMap") as MapFragment;
            if (_mapFragment == null)
            {
                GoogleMapOptions mapOptions = new GoogleMapOptions()
                    .InvokeMapType(GoogleMap.MapTypeNormal)
                    .InvokeZoomControlsEnabled(true)
                    .InvokeCompassEnabled(true);

                FragmentTransaction fragTx = FragmentManager.BeginTransaction();
                _mapFragment = MapFragment.NewInstance(mapOptions);
                fragTx.Add(Resource.Id.internetMap, _mapFragment, "internetMap");
                fragTx.Commit();
            }
        }

        private bool SetupMapIfNeeded()
        {
            if (_map == null)
            {
                _map = _mapFragment.Map;
                if (_map != null)
                {
                    return true;
                }
                return false;
            }
            return true;
        }

		private void StartLocator()
		{
			if (TestIfGooglePlayServicesIsInstalled ()) {
				_locationClient.Connect();
			}
		}

        private void CentreOnLocation(Location location)
        {
            if (_map == null || location == null)
            {
                return;
            }

            LatLng loc = new LatLng(location.Latitude, location.Longitude);
            _map.MoveCamera(CameraUpdateFactory.NewLatLng(loc));
        }

        private void RegisterForLocationUpdates()
        {
            LocationRequest request = new LocationRequest();
            request.SetPriority(100);
            request.SetFastestInterval(5000);
            request.SetInterval(3000);
            request.SetSmallestDisplacement(20);

            _locationClient.RequestLocationUpdates(request, this);
        }

        /// <summary>
        /// Tests to see whether or not Google Play Services is installed on the device.
        /// </summary>
        /// <returns>A value indicating whether or not the services are installed.</returns>
        private bool TestIfGooglePlayServicesIsInstalled()
        {
            const string tag = "Initialization";
            const int installGooglePlayServicesId = 1000;
            int queryResult = GooglePlayServicesUtil.IsGooglePlayServicesAvailable(this);

            if (queryResult == ConnectionResult.Success)
            {
                Log.Info(tag, "Google Play Services is installed on this device.");
                return true;
            }

            if (GooglePlayServicesUtil.IsUserRecoverableError(queryResult))
            {
                string errorString = GooglePlayServicesUtil.GetErrorString(queryResult);
                Log.Error(tag, "There is a problem with Google Play Services on this device: {0} - {1}", queryResult, errorString);
                Dialog errorDialog = GooglePlayServicesUtil.GetErrorDialog(queryResult, this, installGooglePlayServicesId);
                errorDialog.Show();
            }

            return false;
        }
    }
}

