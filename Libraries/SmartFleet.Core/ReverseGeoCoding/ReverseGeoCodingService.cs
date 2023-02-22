using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using SmartFleet.Core.Contracts.Commands;
using SmartFleet.Core.Domain.Movement;
using SmartFleet.Core.ReverseGeoCoding.Dtos;

namespace SmartFleet.Core.ReverseGeoCoding
{
    public class ReverseGeoCodingService
    {
        private const string KEY = "pk.cc7d7c232c3b43aa3a87127b93b22339";
        private  string _locationiqUrl = ConfigurationManager.AppSettings["locationiqUrl"];
        private string _nominatimUrl = ConfigurationManager.AppSettings["nominatimUrl"];
        private int count = 0;
        private SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);
        private readonly string[] _userAgents = { "Mozilla/4.0 (Mozilla/4.0; MSIE 7.0; Windows NT 5.1; FDM; SV1)"
            , "Mozilla/4.0 (Mozilla/4.0; MSIE 7.0; Windows NT 5.1; FDM; SV1; .NET CLR 3.0.04506.30)",
            "Mozilla/4.0 (Windows; MSIE 7.0; Windows NT 5.1; SV1; .NET CLR 2.0.50727)",
            "Mozilla/4.0 (Windows; U; Windows NT 5.0; en-US) AppleWebKit/532.0 (KHTML, like Gecko) Chrome/3.0.195.33 Safari/532.0",
            "Mozilla/4.0 (Windows; U; Windows NT 5.1; en-US) AppleWebKit/525.19 (KHTML, like Gecko) Chrome/1.0.154.59 Safari/525.19",
            "Mozilla/4.0 (compatible; MSIE 6.0; Linux i686 ; en) Opera 9.70",
            "Mozilla/4.0 (compatible; MSIE 6.0; Mac_PowerPC; en) Opera 9.24",
            "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; de) Opera 9.50",
            "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; en) Opera 9.24",
            "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; en) Opera 9.26",
            "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; es-la) Opera 9.27",
            "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0; YPC 3.2.0; SLCC1; .NET CLR 2.0.50727; .NET CLR 3.0.04506)"
        };
        public async Task ReverseGeoCodingAsync(CreateTk103Gps gpsStatement)
        {
            var lat = gpsStatement.Latitude.ToString(CultureInfo.InvariantCulture).Replace(",", ".");
            var lon = gpsStatement.Longitude.ToString(CultureInfo.InvariantCulture).Replace(",", ".");
            var client = new HttpClient();
            var url = $"{_locationiqUrl}{KEY}&lat={lat}&lon={lon}&format=json";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
           
            if (response.IsSuccessStatusCode)
            {
                var r = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var result = JsonConvert.DeserializeObject<LocationiqResponse>(r);
                gpsStatement.Address = result.display_name;
                gpsStatement.Region = result.address.region;
                gpsStatement.State = result.address.state;
            }

        }
        public async Task ReverseGeoCodingAsync(Position gpsStatement)
        {
            var lat = gpsStatement.Lat.ToString(CultureInfo.InvariantCulture).Replace(",", ".");
            var lon = gpsStatement.Long.ToString(CultureInfo.InvariantCulture).Replace(",", ".");
            var client = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var url = $"{_locationiqUrl}{KEY}&lat={lat}&lon={lon}&format=json";
            HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var r = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var result = JsonConvert.DeserializeObject<LocationiqResponse>(r);
                gpsStatement.Address = result.display_name;
                gpsStatement.Region = result.address.region;
                gpsStatement.State = result.address.state;
            }

        }

        public async Task<string> ReverseGeoCodingAsync(double Lat, double Long)
        {
            var lat = Lat.ToString(CultureInfo.InvariantCulture).Replace(",", ".");
            var lon = Long.ToString(CultureInfo.InvariantCulture).Replace(",", ".");
            var client = new HttpClient();
            var url = $"{_locationiqUrl}{KEY}&lat={lat}&lon={lon}&format=json";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var r = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var locationiqResponse = JsonConvert.DeserializeObject<LocationiqResponse>(r);
                return locationiqResponse.display_name;
            }

            return null;
        }

        public  async Task<NominatimResult> ExecuteQueryAsync(double lat, double lng)
        {
            var client = new RestClient();
            var _lat =lat. ToString(CultureInfo.InvariantCulture).Replace(",", ".");
            var _lon = lng.ToString(CultureInfo.InvariantCulture).Replace(",", ".");
            count++;
            Debug.WriteLine(count);
            var url =  $"https://nominatim.openstreetmap.org/reverse.php?format=json&lat={_lat}&lon={_lon}";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var request = new RestRequest(Method.GET);
            //request.Resource = "wsRest/wsServerArticle/getArticle";
            client.BaseUrl = new System.Uri(url);
            var rd = new Random();
            client.UserAgent = _userAgents[rd.Next(0, _userAgents.Length)]; 
            try
            {
                
                var response = await client.ExecutePostTaskAsync<NominatimResult>(request).ConfigureAwait(false);
                if (response.ErrorException != null)
                {
                    const string message = "Error retrieving response.  Check inner details for more info.";
                    throw new ApplicationException(message, response.ErrorException);
                }

                return response.Data;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
        public  async Task<string> ReverseGeocodeAsync(double lat, double log)
        {

            string add;
            do
            {

                try
                {
                    await _semaphoreSlim.WaitAsync(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
                    var r = await ExecuteQueryAsync(lat, log).ConfigureAwait(false);
                    _semaphoreSlim.Release();
                    if (r?.display_name != null && !string.IsNullOrEmpty(r.display_name))
                    {
                        Thread.Sleep(1000);
                        return r.display_name;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    _semaphoreSlim.Release();
                    Thread.Sleep(1000);
                }
                
                add = await ReverseGeoCodingAsync(lat, log)
                    .ConfigureAwait(false);
                Thread.Sleep(1000);
                if (add != string.Empty && !string.IsNullOrEmpty(add))
                     return add;
                 
            } while (add == string.Empty);

            return add;
        }

    }
}
