using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using Microsoft.VisualBasic.FileIO;
namespace WoWDataMigrate
{
    static class Database
    {
        public enum ErrorType
        {
            All,
            Item,
            Boss
        }
    
        public static void WriteItemToJson(Item item)
        {
            var existingData = File.ReadAllText(@"D:\Coding Projects\my-app\items.json");
            var itemList = JsonConvert.DeserializeObject<List<Item>>(existingData)
                ?? new List<Item>();
            itemList.Add(item);
            
            string json = JsonConvert.SerializeObject(itemList.ToArray());
            System.IO.File.WriteAllText(@"D:\Coding Projects\my-app\items.json", json);
        }
        //public static void WriteItemsToJson(Dictionary<int, List<ItemJson>> dict)
        //{
        //    List<TrueItemJson> list = new List<TrueItemJson>();
        //    foreach (KeyValuePair<int,List<ItemJson>> kvp in dict)
        //    {
        //        kvp.Value.ForEach(x => list.Add(new TrueItemJson(x, kvp.Key)));
        //    }
            

        //    var itemData = System.IO.File.ReadAllText(@"D:\Coding Projects\my-app\itemJson.txt");
        //    var itemList = JsonConvert.DeserializeObject<List<TrueItemJson>>(itemData)
        //                    ?? new List<TrueItemJson>();

        //    var fullItemList = itemList.Concat(list).ToList();

        //    string json = JsonConvert.SerializeObject(fullItemList.ToArray());
        //    System.IO.File.WriteAllText(@"D:\Coding Projects\my-app\itemJson.txt", json);
        //    WriteItemStatsToJson(dict);
        //}

        //public static void WriteItemStatsToJson(Dictionary<int, List<ItemJson>> dict)
        //{
        //    List<TrueItemStatJson> list = new List<TrueItemStatJson>();
        //    foreach (KeyValuePair<int, List<ItemJson>> kvp in dict)
        //    {
        //        kvp.Value.ForEach(x =>
        //            x.bonusStats.ForEach(y => list.Add(new TrueItemStatJson(x.id, y))));
        //    }

        //    var itemStatData = File.ReadAllText(@"D:\Coding Projects\my-app\itemStatJson.txt");
        //    var itemStatList = JsonConvert.DeserializeObject<List<TrueItemStatJson>>(itemStatData)
        //                    ?? new List<TrueItemStatJson>();

        //    var fullItemStatList = itemStatList.Concat(list).ToList();
        //    string json = JsonConvert.SerializeObject(fullItemStatList.ToArray());

        //    System.IO.File.WriteAllText(@"D:\Coding Projects\my-app\itemStatJson.txt", json);
        //}

        public static void WriteZonesToJson(List<Zone> zones)
        {
            List<TrueZoneJson> list = new List<TrueZoneJson>();
            foreach(Zone zone in zones)
            {
                list.Add(new TrueZoneJson(zone));
            }
            string json = JsonConvert.SerializeObject(list.ToArray());

            System.IO.File.WriteAllText(@"D:\Coding Projects\my-app\zoneJson.txt", json);
        }

        public static void WriteBossesToJson(List<Zone> zones)
        {
            List<TrueBossJson> bosses = new List<TrueBossJson>();
            foreach(Zone zone in zones)
            {
                foreach(Boss boss in zone.bosses)
                {
                    boss.CorrectId();
                    bosses.Add(new TrueBossJson(boss, zone));
                }
            }
            string json = JsonConvert.SerializeObject(bosses.ToArray());

            System.IO.File.WriteAllText(@"D:\Coding Projects\my-app\bossJson.json", json);

        }
        public static List<int> GetCompleteBosses()
        {
            using (StreamReader r = new StreamReader(@"D:\Coding Projects\my-app\completeBosses.json"))
            {
                string text = r.ReadToEnd();
                List<string> list = text.Split(',').ToList();
                List<int> intList = list.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => int.Parse(x)).ToList();
                return intList;
            }
        }
        public static void WriteCompleteBossToJson(int bossId)
        {
            System.IO.File.AppendAllText(@"D:\Coding Projects\my-app\completeBosses.json", bossId.ToString()+",");
        }

        
        public static List<TrueBossJson> GetBosses()
        {
            using (StreamReader r = new StreamReader(@"D:\Coding Projects\my-app\bossJson.json"))
            {
                string json = r.ReadToEnd();
                List<TrueBossJson> bosses = JsonConvert.DeserializeObject<List<TrueBossJson>>(json);
                return bosses.ToList();
            }
        }
            //public static List<int> BossIds = new List<int> {
            //  122967,
            //  122965,
            //  122963,
            //  122968,
            //  126832,
            //  129431,
            //  126969,
            //  126983,
            //  127479,
            //  127484,
            //  127490,
            //  127503,
            //  129214,
            //  129227,
            //  129231,
            //  131227,
            //  135360,
            //  131667,
            //  131863,
            //  131527,
            //  144324,
            //  131318,
            //  131817,
            //  131383,
            //  133007,
            //  128650,
            //  129208,
            //  128651,
            //  128652,
            //  133379,
            //  133384,
            //  133389,
            //  133392,
            //  134056,
            //  134063,
            //  134060,
            //  134069,
            //  135322,
            //  134993,
            //  135470,
            //  136160,
            //  137119,
            //  140853,
            //  133298,
            //  134445,
            //  134442,
            //  138967,
            //  136383,
            //  132998,
            //  144683,
            //  144680,
            //  148238,
            //  148117,              
            //  145261,
            //  144747,
            //  145616,
            //  144796,
            //  146256,
            //  149684
            //};
    }

    public class TrueZoneJson
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool isRaid { get; set; }

        public TrueZoneJson(Zone zone)
        {
            id = zone.id;
            name = zone.name;
            isRaid = zone.isRaid;
        }
    }

    public class TrueBossJson
    {        
        public string name { get; set; }
        public int id { get; set; }
        public int zoneId { get; set; }
        
        public TrueBossJson()
        {

        }
        public TrueBossJson(Boss boss, Zone zone)
        {            
            id = boss.id;
            name = boss.name;
            zoneId = zone.id;
        }
    }

    public class TrueItemJson
    {
        public int id { get; set; }
        public string name { get; set; }
        public int sourceId { get; set; }
        public int inventoryType { get; set; }
        public int itemLevel { get; set; }

        public TrueItemJson()
        {

        }
        public TrueItemJson(ItemJson itemJson, int bossId)
        {
            id = itemJson.id;
            name = itemJson.name;
            inventoryType = itemJson.inventoryType;
            sourceId = bossId;
            itemLevel = itemJson.itemLevel;
        }
    }
    public class TrueItemStatJson
    {
        public int itemId { get; set; }
        public int stat { get; set; }
        public int amount { get; set; }
        
        public TrueItemStatJson()
        {

        }
        public TrueItemStatJson(int _itemId, BonusStat bonusStat)
        {
            itemId = _itemId;
            stat = bonusStat.stat;
            amount = bonusStat.amount;
        }
    }

    public class Item
    {
        [JsonProperty("name")]
        public string name;
        [JsonProperty("id")]
        public int id;
        [JsonProperty("bonusStats")]
        public List<Stat> stats;
        [JsonProperty("inventoryType")]
        public int inventoryType;
        [JsonProperty("itemLevel")]
        public int itemLevel;
        public int sourceId;
    }

    public class Stat
    {
        [JsonProperty("stat")]
        public int id;
        [JsonProperty("amount")]
        public int amount;
    }
}
