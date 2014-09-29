using System;
using MonoTouch.UIKit;
using MonoTouch.MapKit;
using System.Drawing;
using MonoTouch.Foundation;

namespace DataVic.iPhone
{
	/// <summary>
	/// A helper class to display the WiFi range on a map overlay.
	/// </summary>
	internal class WiFiRangeImageHelper
	{
		private readonly UIImage _wifiRangeImage;
		private readonly MKMapRect _mapBounds;

		public WiFiRangeImageHelper(object overlay, UIImage image)
		{
			_wifiRangeImage = image;
			_mapBounds = ((MKCircle)overlay).BoundingMapRect;
		}

		public void DrawMapRect (MKMapRect mapRect, MonoTouch.CoreGraphics.CGContext context, Func<MKMapRect, RectangleF> calculateDisplayRectangle)
		{
			RectangleF bounds = calculateDisplayRectangle(_mapBounds);
			context.ScaleCTM (1, -1);
			context.TranslateCTM (0, -bounds.Height);
			context.DrawImage (bounds, _wifiRangeImage.CGImage);
		}
	}
}

