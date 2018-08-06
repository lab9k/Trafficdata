using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace RemoteApi
{
    public class RestClient
    {
        private static readonly HttpClient Client = new HttpClient();

        public static async Task<string> Get(string url, Dictionary<string,string> queryParams = null)
        {

            if (queryParams != null)
            {
                var builder = new UriBuilder(url);
                builder.Port = -1;
                var query = HttpUtility.ParseQueryString(builder.Query);
                foreach (var kvp in queryParams)
                {
                    query.Add(kvp.Key.ToString(), kvp.Value.ToString());
                }
                builder.Query = query.ToString();
                url = builder.ToString();
            }

            var stringTask = Client.GetStringAsync(url);

            return stringTask.Result;

        }
    }
}
