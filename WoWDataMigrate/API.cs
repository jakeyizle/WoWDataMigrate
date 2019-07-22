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
        const string apiKey = "US1rNM1638PTPiSE6stHVbS5Rq5r85AjQN";
        const string baseUrl = "https://us.api.blizzard.com/";
        const string locale = "en_US";

        static RestClient client = new RestClient
        {
            BaseUrl = new Uri(baseUrl)
        };

        public static IRestResponse GetItem(int itemId, TrueBossJson boss = null)
        {
            //this is a hacky workaround - there is "junk loot" attached to dungeon bosses which is filterable by ilvl
            //however when we call with the dungeon bonusId, the junk loot itemlevel is higher than some of the raids
            //so, call the base item first and then if it passes we call it again with the proper bonusId
            //Perhaps its possible to filter by item rarity (get rid of all uncommon/green)
            if (boss == null)
            {
                RestRequest baseRequest = new RestRequest
                {
                    Resource = $"wow/item/{itemId}?locale={locale}&access_token={apiKey}"
                };
                return client.Execute(baseRequest);
            }
            //Items arent returned correctly from API by default
            //A bonusId must be supplied to determine the itemlevel. Mythic is used for raids and M10 for dungeons
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
                case 10057:
                    bonusId = 1547;
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
            RestRequest request = new RestRequest
            {
                Resource = $"wow/item/{itemId}?locale={locale}&access_token={apiKey}&bl={bonusId}"
            };
            return client.Execute(request);
        }

        public static IRestResponse GetAllZones()
        {
            RestRequest request = new RestRequest
            {
                Resource = $"wow/zone/?locale={locale}&access_token={apiKey}"
            };
            return client.Execute(request);
        }
    }
}
