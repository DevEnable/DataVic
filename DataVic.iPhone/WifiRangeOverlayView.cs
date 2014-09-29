using System;
using MonoTouch.MapKit;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Foundation;

namespace DataVic.iPhone
{
	internal class WifiRangeOverlayView : MKOverlayView
	{
		private readonly WiFiRangeImageHelper _helper;

		public WifiRangeOverlayView (NSObject overlay, UIImage image) : base(overlay)
		{
			_helper = new WiFiRangeImageHelper (overlay, image);
		}

		public override void DrawMapRect (MKMapRect mapRect, float zoomScale, MonoTouch.CoreGraphics.CGContext context)
		{
			_helper.DrawMapRect (mapRect, context, this.RectForMapRect);
		}
	}
}

