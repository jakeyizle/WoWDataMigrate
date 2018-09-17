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
        static void Main()
        {
           switch(UI.Text("1. Item Migration " +
               "\r\n 2. Zone Migration" +
               "\r\n 3. Boss Migration").ToLower())
            {
                case "1":
                    MigrateItems();
                    Main();
                    break;
                case "2":
                    MigrateZones();
                    Main();
                    break;
                case "3":
                    Main();
                    break;
                default:
                    Main();
                    break;
            }
        }

        static void MigrateItems()
        {
            List<int> idList = Database.GetItemIds().Concat(Database.GetErrorIds(Database.ErrorType.Item)).ToList();
            for (int itemId = 200000; itemId < 0; itemId--)
            {
                if (idList.Contains(itemId))
                {
                    continue;
                }
                IRestResponse response = API.GetItem(itemId);

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
                SleepIfNeeded(response);

            }
        }
        static void MigrateZones()
        {
            IRestResponse response = API.GetAllZones();
            ZoneJson zoneJson = JsonConvert.DeserializeObject<ZoneJson>(response.Content);
            foreach (Zone zone in zoneJson.zones)
            {
                if (zone.expansionId == 7)
                {
                    Database.InsertZone(zone);
                    Database.InsertBossesFromZone(zone);
                }
            }
        }
        static void SleepIfNeeded(IRestResponse response)
        {
            //Sleep if approaching API limit
            if (response.Headers.ToList()[7].ToString().Contains("100"))
            {
                System.Threading.Thread.Sleep(1050);
            }
            if (response.Headers.ToList()[9].ToString().Contains("36000"))
            {
                System.Threading.Thread.Sleep(3600050);
            }
        }
    }
}
