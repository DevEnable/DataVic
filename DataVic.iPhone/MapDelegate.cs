using MonoTouch.Foundation;
using MonoTouch.MapKit;
using MonoTouch.UIKit;
using System.Drawing;

namespace DataVic.iPhone
{
    public partial class MainViewController
    {
        private class MapDelegate : MKMapViewDelegate
        {
			private static readonly UIImage WifiIcon = GetScaledImage("WiFi.png", 32, 32);
            private static readonly UIImage WifiRange = UIImage.FromBundle("WiFiArea.png");

            public override MKAnnotationView GetViewForAnnotation(MKMapView mapView, NSObject annotation)
            {
                InternetAnnotation internetAnnotation = annotation as InternetAnnotation;

                if (internetAnnotation == null)
                {
					return new MKPinAnnotationView(annotation, "currentLocation");    
                }

                MKAnnotationView view;

                if (internetAnnotation.HasWiFi)
                {
					view = mapView.DequeueReusableAnnotation("WiFi") ?? new MKAnnotationView(annotation, "WiFi");
					view.Image = WifiIcon;
					view.CanShowCallout = true;
					view.RightCalloutAccessoryView = UIButton.FromType (UIButtonType.DetailDisclosure);
                }
                else
                {	
					view = mapView.DequeueReusableAnnotation("NoWiFi") ?? new MKPinAnnotationView(annotation, "NoWiFi");
					view.CanShowCallout = true;
					view.RightCalloutAccessoryView = UIButton.FromType (UIButtonType.DetailDisclosure);
                }

                return view;
            }

			public override MKOverlayRenderer OverlayRenderer(MKMapView mapView, IMKOverlay overlay)
			{
				return new WiFiRangeOverlayRenderer (overlay, WifiRange);
			}

			public override MKOverlayView GetViewForOverlay(MKMapView mapView, NSObject overlay)
            {
				return new WifiRangeOverlayView (overlay, WifiRange);
			}

			private static UIImage GetScaledImage(string imageName, int x, int y)
			{
				// I am cheating here as I know both of my images are square so I do not need to worry about distorting the aspect ratio
				UIImage image = UIImage.FromBundle (imageName);
				UIGraphics.BeginImageContext (new SizeF (x, y));
				image.Draw (new RectangleF (0, 0, x, y));
				image = UIGraphics.GetImageFromCurrentImageContext ();
				UIGraphics.EndImageContext ();

				return image;
			}

        }
    }
}