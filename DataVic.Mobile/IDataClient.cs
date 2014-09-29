using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataVic.Mobile
{
    public interface IDataClient
    {
        Task<IEnumerable<InternetLocation>> GetLocations(double latitude, double longitude);
    }
}
