using System;
using Microsoft.WindowsAzure.MobileServices;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Globalization;

namespace DataVic.Mobile
{
	public class DataClient : IDataClient
	{
		private readonly MobileServiceClient _client = new MobileServiceClient("https://datavic.azure-mobile.net/", "yourkeyhere");

		public Task<IEnumerable<InternetLocation>> GetLocations(double latitude, double longitude)
		{
			return _client.GetTable<InternetLocation>().WithParameters(
				new Dictionary<string, string>
				{
					{"longitude", longitude.ToString(CultureInfo.InvariantCulture)},
					{"latitude", latitude.ToString(CultureInfo.InvariantCulture)}
				}).ToEnumerableAsync();

		}
	}
}

