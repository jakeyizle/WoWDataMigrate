using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;

namespace WoWDataMigrate
{
    class Program
    {
        static void Main(string[] args)
        {
            string apiKey = "qhgexm8jckvmr3xfdczeqmcb8mnwb6an";
            string baseUrl = "https://us.api.battle.net/";
            DateTime previousCall = DateTime.Now;
            RestClient client = new RestClient
            {
                BaseUrl = new Uri(baseUrl)
            };
            List<int> idList = Database.GetItemIds().Concat(Database.GetErrorIds(Database.ErrorType.Item)).ToList();
            
            for (int i = 0; i < 1000000; i ++)
            {
                int itemId = i;
                if (idList.Contains(itemId))
                {
                    continue;
                }
                RestRequest request = new RestRequest
                {
                    Resource = $"wow/item/{itemId}?locale=en_US&apikey={apiKey}"
                };

                while ((DateTime.Now - previousCall).TotalSeconds < 1.5)
                {
                    System.Threading.Thread.Sleep(200);
                }
                {
                    IRestResponse response = client.Execute(request);

                    if (!response.StatusCode.ToString().Contains("OK"))
                    {
                        Database.InsertError(Database.ErrorType.Item, itemId);
                    }
                    else
                    {
                        ItemJson itemJson = JsonConvert.DeserializeObject<ItemJson>(response.Content);
                        Database.InsertItem(itemJson);
                        Console.WriteLine("ItemId: " + itemJson.id + " ItemName: " + itemJson.name);
                    }
                }
                previousCall = DateTime.Now;
            }
        }
    }
}
