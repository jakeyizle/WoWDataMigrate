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
            List<TrueBossJson> bossList = Database.GetBosses();          
            List<int> completedBosses = Database.GetCompleteBosses();
            foreach (TrueBossJson boss in bossList)
            {
                if (completedBosses.Count != 0 && completedBosses.Contains(boss.id))
                {
                    continue;
                }

                Dictionary<int, List<ItemJson>> dict = new Dictionary<int, List<ItemJson>>();
                List<ItemJson> list = new List<ItemJson>();
                List<int> itemIds = Protractor.GetItemIds(boss.id).ToList();
                foreach (int itemId in itemIds)
                {
                    //Verify base item level is high enough to not be a junk item
                    IRestResponse response = API.GetItem(itemId);
                    try
                    {
                        ItemJson itemJson = JsonConvert.DeserializeObject<ItemJson>(response.Content);

                    itemJson.itemSource.sourceId = boss.id;
                    if (itemJson.itemLevel < 285)
                    {
                        Console.WriteLine("BAD ITEM - ItemId: " + itemJson.id + " ItemName: " + itemJson.name);
                        continue;
                    }
                    //item isn't junk, so get it at the proper itemlevel
                    response = API.GetItem(itemId, boss);
                    itemJson = JsonConvert.DeserializeObject<ItemJson>(response.Content);
                    list.Add(itemJson);
                    Console.WriteLine($"ItemId: {itemJson.id} - ItemName: {itemJson.name} - ItemLevel - {itemJson.itemLevel}");
                    }
                    catch (Exception)
                    {
                        Console.WriteLine($"ERROR: {itemId}");
                    }
                }
                dict.Add(boss.id, list);
                Database.WriteItemsToJson(dict);
                Database.WriteCompleteBossToJson(boss.id);
            }


            Protractor.Teardown();
        }
        
        static void MigrateZones()
        {
            IRestResponse response = API.GetAllZones();
            ZoneJson zoneJson = JsonConvert.DeserializeObject<ZoneJson>(response.Content);
            List<Zone> zones = new List<Zone>();
            foreach (Zone zone in zoneJson.zones)
            {
                if (zone.expansionId == 7)
                {
                    zones.Add(zone);
                    //Database.InsertZone(zone);
                    //Database.InsertBossesFromZone(zone);
                }
            }
            Database.WriteZonesToJson(zones);
            Database.WriteBossesToJson(zones);
        }
    }
}
