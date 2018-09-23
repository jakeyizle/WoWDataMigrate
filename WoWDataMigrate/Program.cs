using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;
using System.Collections;

namespace WoWDataMigrate
{
    class Program
    {

        static void Main()
        {
           switch(UI.Text("1. Item Migration " +
               "\r\n2. Zone and Boss Migration").ToLower())
            {
                case "1":
                    MigrateItems();
                    Main();
                    break;
                case "2":
                    MigrateZones();
                    Main();
                    break;
                default:
                    Main();
                    break;
            }
        }

        static void MigrateItems()
        {
            List<int> bossList = Database.GetBossIds();
            List<int> existingItems = Database.GetBadItemIds().Concat(Database.GetItemIds()).ToList();

            foreach (int bossId in bossList)
            {
                List<int> itemIds = Protractor.GetItemIds(bossId).Except(existingItems).ToList();
                foreach (int itemId in itemIds)
                {
                    IRestResponse response = API.GetItem(itemId);
                    ItemJson itemJson = JsonConvert.DeserializeObject<ItemJson>(response.Content);
                    itemJson.itemSource.sourceId = bossId;
                    if (itemJson.itemLevel < 285)
                    {
                        Database.InsertBadItem(itemJson);
                        Console.WriteLine("BAD ITEM - ItemId: " + itemJson.id + " ItemName: " + itemJson.name);
                        continue;
                    }
                    Database.InsertItem(itemJson);
                    Console.WriteLine("ItemId: " + itemJson.id + " ItemName: " + itemJson.name);
                }
            }
            Protractor.Teardown();
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
    }
}
