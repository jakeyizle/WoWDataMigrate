using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

                List<int> itemIds = Protractor.GetItemIds(boss.id).ToList();
                foreach (int itemId in itemIds)
                {
                    //Verify base item level is high enough to not be a junk item
                    IRestResponse response = API.GetItem(itemId);
                    try
                    {
                        ItemJson itemJson = JsonConvert.DeserializeObject<ItemJson>(response.Content);
                    
                        if (itemJson.itemLevel < 285)
                        {
                            Console.WriteLine("BAD ITEM - ItemId: " + itemJson.id + " ItemName: " + itemJson.name);
                            continue;
                        }
                        //item isn't junk, so get it at the proper itemlevel
                        //get base item and then +5, +10, and +15 versions                        
                        for (int i = 0; i > 4; i++)
                        {
                            int bonusId = GetBonusId(boss) + i*5;
                            response = API.GetItem(itemId, bonusId);
                            JObject test = JObject.Parse(response.Content);
                            Item item = test.ToObject<Item>();
                            item.sourceId = boss.id;
                            Database.WriteItemToJson(item);
                            Console.WriteLine($"ItemId: {item.id} - ItemName: {item.name} - ItemLevel - {item.itemLevel}");
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine($"ERROR: {itemId}");
                    }
                }
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
                }
            }
            Database.WriteZonesToJson(zones);
            Database.WriteBossesToJson(zones);
        }

        static int GetBonusId(TrueBossJson boss)
        {
            int bonusId;
            switch (boss.zoneId)
            {
                //Uldir
                case 9389:
                    bonusId = 1507;
                    break;
                //BoD
                case 8670:
                    bonusId = 1537;
                    break;
                //Crucible
                //Not correct - 1st boss and 2nd drop different ilvls. Check boss id here too?
                //1st boss is 420
                //2nd boss is 425
                case 10057:                    
                    bonusId = boss.id == 145371 ? 1547 : 1542;
                    break;
                //Palace
                case 10425:
                    bonusId = 1517;
                    break;
                //Dungeon
                //Season 3 M10 is 430
                default:
                    bonusId = 1602;
                    break;
            }
            return bonusId;
        }
    }
}
