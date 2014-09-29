using System;
using MonoTouch.MapKit;
using MonoTouch.UIKit;
using System.Drawing;

namespace DataVic.iPhone
{
	internal class WiFiRangeOverlayRenderer : MKOverlayRenderer
	{
		private readonly WiFiRangeImageHelper _helper;

		public WiFiRangeOverlayRenderer(IMKOverlay overlay, UIImage image) : base(overlay)
		{
			_helper = new WiFiRangeImageHelper (overlay, image);
		}

		public override void DrawMapRect (MKMapRect mapRect, float zoomScale, MonoTouch.CoreGraphics.CGContext context)
		{
			_helper.DrawMapRect (mapRect, context, this.RectForMapRect);
		}
	}
}

