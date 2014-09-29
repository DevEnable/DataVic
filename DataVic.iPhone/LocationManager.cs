using System;

using MonoTouch.CoreLocation;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace DataVic.iPhone
{
	/// <summary>
	/// Copy / Paste from the Xamarin Core Sample
	/// </summary>
	public class LocationManager
	{
		protected CLLocationManager _locationManager; 

		// event for the location changing
		public event EventHandler<LocationUpdatedEventArgs> LocationUpdated = delegate { };

		public LocationManager ()
		{
			this._locationManager = new CLLocationManager();
		}

		// create a location manager to get system location updates to the application
		private CLLocationManager CLLocationManager
		{
			get 
			{ 
				return this._locationManager; 
			} 
		} 

		public void StartLocationUpdates ()
		{
			// We need the user's permission for our app to use the GPS in iOS. This is done either by the user accepting
			// the popover when the app is first launched, or by changing the permissions for the app in Settings

			if (CLLocationManager.LocationServicesEnabled) {

				CLLocationManager.DesiredAccuracy = 20; // sets the accuracy that we want in meters
				CLLocationManager.DistanceFilter = 20;

				// Location updates are handled differently pre-iOS 6. If we want to support older versions of iOS,
				// we want to do perform this check and let our LocationManager know how to handle location updates.

				if (UIDevice.CurrentDevice.CheckSystemVersion (6, 0)) {

					CLLocationManager.LocationsUpdated += (object sender, CLLocationsUpdatedEventArgs e) => {
						// fire our custom Location Updated event
						this.LocationUpdated (this, new LocationUpdatedEventArgs (e.Locations [e.Locations.Length - 1]));
					};

				} else {

					// this won't be called on iOS 6 (deprecated). We will get a warning here when we build.
					CLLocationManager.UpdatedLocation += (object sender, CLLocationUpdatedEventArgs e) => {
						this.LocationUpdated (this, new LocationUpdatedEventArgs (e.NewLocation));
					};
				}

				// Start our location updates
				CLLocationManager.StartUpdatingLocation ();

				// Get some output from our manager in case of failure
				CLLocationManager.Failed += (object sender, NSErrorEventArgs e) => {
					Console.WriteLine (e.Error);
				}; 

			} 
			else 
			{
				//Let the user know that they need to enable LocationServices
				Console.WriteLine ("Location services not enabled, please enable this in your Settings");

			}
		}

	}
}

