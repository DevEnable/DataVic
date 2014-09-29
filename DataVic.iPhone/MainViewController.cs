using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreLocation;
using MonoTouch.MapKit;
using System.Collections.Generic;
using DataVic.Mobile;
using System.Linq;

namespace DataVic.iPhone
{
	public partial class MainViewController : UIViewController
	{
		private readonly DataClient _client = new DataClient();

	    private MKPointAnnotation _currentUserLocation; 

		// Constructor invoked from the NIB loader
		public MainViewController (IntPtr p) : base(p)
		{

		}

		public override void ViewDidLoad ()
		{
			// call your base
			base.ViewDidLoad ();

			this.InternetLocationsMap.Delegate = new MapDelegate ();
					
			// It is better to handle this with notifications, so that the UI updates
			// resume when the application re-enters the foreground!

			// screen subscribes to the location changed event
			UIApplication.Notifications.ObserveDidBecomeActive ((sender, args) => {
				AppDelegate.LocationManager.LocationUpdated += OnLocationChanged;
			});

			// whenever the app enters the background state, we unsubscribe from the event 
			// so we no longer perform foreground updates
			UIApplication.Notifications.ObserveDidEnterBackground ((sender, args) => {
				AppDelegate.LocationManager.LocationUpdated -= OnLocationChanged;
			});
		}

		public async void OnLocationChanged (object sender, LocationUpdatedEventArgs e)
		{
			// handle foreground updates
			CLLocation location = e.Location;

			try
			{
				var locationsTask = _client.GetLocations(location.Coordinate.Latitude, location.Coordinate.Longitude);
				InvokeOnMainThread(() => CenterOnLocation(location.Coordinate));  
				IList<InternetLocation> locations = (await locationsTask).ToList();
				Console.WriteLine("{0} nearby Internet locations found, {1} of which have WiFi.", locations.Count, locations.Count(l => l.HasWifi));
				InvokeOnMainThread(() => MarkInternetLocations(locations));  
			}
			catch(Exception ex) 
			{
				// TODO: Log the error.
				throw;
			}
			finally
			{
				Console.WriteLine ("foreground updated");
			}
		}

		private void CenterOnLocation(CLLocationCoordinate2D location)
		{
			MKCoordinateRegion region = new MKCoordinateRegion
			{
			    Center = location,
			    Span =
			    {
					LatitudeDelta = 0.1f, 
					LongitudeDelta = 0.1f
				}
			};

		    // Can also use .SetCenterCoordinate which takes the CLLocationCoordinate2D object, but won't allow for the zoom span.
			this.InternetLocationsMap.SetRegion (region, true);

            UpdateCurrentLocation(location);
		}

	    private void UpdateCurrentLocation(CLLocationCoordinate2D location)
	    {
            bool firstLocation = _currentUserLocation == null;

            if (firstLocation)
            {
                _currentUserLocation = new MKPointAnnotation();
            }
            
            /* Need a custom annotation as we do not want to use the LocationManager twice.  If you are using
               ShowUserLocation then check for MKUserLocation before clearing annotations */ 

            _currentUserLocation.Title = "You are here";
            _currentUserLocation.Coordinate = location;

            if (firstLocation)
            {
				this.InternetLocationsMap.AddAnnotation(_currentUserLocation);
            }
	    }

		private void MarkInternetLocations(IEnumerable<InternetLocation> locations)
		{
            // Remove all of the annotations apart from the current user location.
			this.InternetLocationsMap.RemoveAnnotations(this.InternetLocationsMap.Annotations.Where(a => a != _currentUserLocation).ToArray());

			// We have already added an annotation for the current user location but may not have necessarily added any overlays.

			if (this.InternetLocationsMap.Overlays != null) 
			{
				this.InternetLocationsMap.RemoveOverlays (this.InternetLocationsMap.Overlays);
			}

		    foreach (InternetLocation location in locations)
		    {
		        DrawAnnotation(location);
		    }
		}

	    private void DrawAnnotation(InternetLocation location)
	    {
            InternetAnnotation annotation = new InternetAnnotation(location);
			this.InternetLocationsMap.AddAnnotation(annotation);

			if (annotation.HasWiFi)
	        {
				MKCircle wifiRange = MKCircle.Circle (annotation.Coordinate, 200);
				this.InternetLocationsMap.AddOverlay (wifiRange);
	        }
	    }
	}
}