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
        static readonly string basePath = Environment.CurrentDirectory;
    
        public static void WriteItemToJson(Item item)
        {
            string path = basePath + "\\items.json";
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();;
            }
            
            var existingData = File.ReadAllText(path);
            var itemList = JsonConvert.DeserializeObject<List<Item>>(existingData)
                ?? new List<Item>();
            itemList.Add(item);
            
            string json = JsonConvert.SerializeObject(itemList.ToArray());
            System.IO.File.WriteAllText(path, json);
        }

        public static void WriteZonesToJson(List<Zone> zones)
        {
            string path = basePath + "\\zones.json";
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();;
            }
            List<TrueZoneJson> list = new List<TrueZoneJson>();
            foreach(Zone zone in zones)
            {
                list.Add(new TrueZoneJson(zone));
            }
            string json = JsonConvert.SerializeObject(list.ToArray());

            System.IO.File.WriteAllText(path, json);
        }

        public static void WriteBossesToJson(List<Zone> zones)
        {
            string path = basePath + "\\bosses.json";
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();;
            }

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

            System.IO.File.WriteAllText(path, json);

        }
        public static List<int> GetCompleteBosses()
        {
            string path = basePath + "\\completeBosses.json";
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();;
            }
            using (StreamReader r = new StreamReader(path))
            {
                string text = r.ReadToEnd();
                List<string> list = text.Split(',').ToList();
                List<int> intList = list.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => int.Parse(x)).ToList();
                return intList;
            }
        }
        public static void WriteCompleteBossToJson(int bossId)
        {
            string path = basePath + "\\completeBosses.json";
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();;
            }
            System.IO.File.AppendAllText(path, bossId.ToString()+",");
        }

        
        public static List<TrueBossJson> GetBosses()
        {
            string path = basePath + "\\bosses.json";
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();;
            }
            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                List<TrueBossJson> bosses = JsonConvert.DeserializeObject<List<TrueBossJson>>(json);
                return bosses;
            }
        }           
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
