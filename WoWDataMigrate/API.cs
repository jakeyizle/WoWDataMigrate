using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;

namespace WoWDataMigrate
{
    public static class API
    {
        static string apiKey = "qhgexm8jckvmr3xfdczeqmcb8mnwb6an";
        static string baseUrl = "https://us.api.battle.net/";
        static string locale = "en_US";

        static RestClient client = new RestClient
        {
            BaseUrl = new Uri(baseUrl)
        };

        public static IRestResponse GetItem(int itemId)
        {
            RestRequest request = new RestRequest
            {
                Resource = $"wow/item/{itemId}?locale={locale}&apikey={apiKey}"
            };
            return client.Execute(request);
        }

        public static IRestResponse GetAllZones()
        {
            RestRequest request = new RestRequest
            {
                Resource = $"wow/zone/?locale={locale}&apikey={apiKey}"
            };
            return client.Execute(request);
        }
    }
}
