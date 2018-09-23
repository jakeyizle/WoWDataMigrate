using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoWDataMigrate
{
    public enum availableMode
    {
        DUNGEON_NORMAL,
        DUNGEON_HEROIC,
        DUNGEON_MYTHIC,
        RAID_FLEX_LFR,
        RAID_FLEX_NORMAL,
        RAID_FLEX_HEROIC,
        RAID_MYTHIC,
        RAID_LFR,
        RAID_10_NORMAL,
        RAID_25_NORMAL,
        RAID_10_HEROIC,
        RAID_25_HEROIC,
        LEGACY_RAID_40,
    
    }

    public class Location
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Location2
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Boss
    {
        public int id { get; set; }
        public string name { get; set; }
        public string urlSlug { get; set; }
        public string description { get; set; }
        public int zoneId { get; set; }
        public bool availableInNormalMode { get; set; }
        public bool availableInHeroicMode { get; set; }
        public int health { get; set; }
        public int heroicHealth { get; set; }
        public int level { get; set; }
        public int heroicLevel { get; set; }
        public int journalId { get; set; }
        public List<object> npcs { get; set; }
        public Location2 location { get; set; }

        public void CorrectId()
        {
            //Mother
            if (id == 13542) { id = 140853; }
            //Mogul Reznak
            if (id == 129232) { id = 131227; }
            //Mythrax
            if (id == 134546) { id = 136383; }
            //Captain Eudora
            if (id == 126848) { id = 129431; }
            //Ring of Booty
            if (id == 0) { id = 126969; }
            //Gorak Tu;
            if (id == 143020) { id = 144324; }
        }
    }

    public class Zone
    {
        public int id { get; set; }
        public string name { get; set; }
        public string urlSlug { get; set; }
        public string description { get; set; }
        public Location location { get; set; }
        public int expansionId { get; set; }
        public string patch { get; set; }
        public string numPlayers { get; set; }
        public bool isDungeon { get; set; }
        public bool isRaid { get; set; }
        public int advisedMinLevel { get; set; }
        public int advisedMaxLevel { get; set; }
        public int advisedHeroicMinLevel { get; set; }
        public int advisedHeroicMaxLevel { get; set; }
        public List<availableMode> availableModes { get; set; }
        public int lfgNormalMinGearLevel { get; set; }
        public int lfgHeroicMinGearLevel { get; set; }
        public int floors { get; set; }
        public List<Boss> bosses { get; set; }
    }

    public class ZoneJson
    {
        public List<Zone> zones { get; set; }
    }
}
