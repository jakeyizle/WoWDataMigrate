using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Collections;

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


        static public string connectionString = @"Server=tcp:wowdb69.database.windows.net,1433;Initial Catalog=WoWDB;Persist Security Info=False;User ID=jakeyizle;Password=Ch!pP3Rr;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
   
        public static void InsertItem(ItemJson itemJson)
        {
            string cmdStr = "Insert Into Item ([ItemId], [ItemName], [ItemLevel], [SourceId], [SourceType], [InventoryTypeId]) Values (@itemId, @itemName, @itemLevel, @sourceId, @sourceType, @inventoryTypeId)";
            List<string> parameterNames = new List<string> { "@itemId", "@itemName", "@itemLevel", "@sourceId", "@sourceType", "@inventoryTypeId" };
            ArrayList parameterValues = new ArrayList { itemJson.id, itemJson.name, itemJson.itemLevel, itemJson.itemSource.sourceId, itemJson.itemSource.sourceType, itemJson.inventoryType };
            ExecuteQuery(cmdStr, parameterNames, parameterValues);

            InsertItemStats(itemJson);
        }

        public static void InsertItemStats(ItemJson itemJson)
        {
            foreach (var bonusStat in itemJson.bonusStats)
            {
                string cmdStr = "Insert Into ItemStat (StatId, ItemId, Amount) Values (@statId, @itemId, @amount)";
                List<string> parameterNames = new List<string> { "@statId", "@itemId", "@amount" };
                ArrayList parameterValues = new ArrayList { bonusStat.stat, itemJson.id, bonusStat.amount };
                ExecuteQuery(cmdStr, parameterNames, parameterValues);
            }
        }

        public static void InsertZone(Zone zone)
        {
            string cmdStr = "Insert Into Zone (ZoneId, ZoneName, isRaid) VALUES (@zoneId, @zoneName, @isRaid)";
            List<string> parameterNames = new List<string> { "@zoneId", "@zoneName", "@isRaid" };
            ArrayList parameterValues = new ArrayList { zone.id, zone.name, zone.isRaid };
            ExecuteQuery(cmdStr, parameterNames, parameterValues);

            foreach (availableMode mode in zone.availableModes)
            {
                cmdStr = "Insert Into ZoneMode (ZoneId, ModeId) VALUES (@zoneId, @modeId)";
                List<string> parameterNames2 = new List<string> { "@zoneId", "@modeId" };
                ArrayList parameterValues2 = new ArrayList { zone.id, mode };
                ExecuteQuery(cmdStr, parameterNames2, parameterValues2);
            }
        }

        public static void InsertBossesFromZone(Zone zone)
        {
            foreach (Boss boss in zone.bosses)
            {
                string cmdStr = "Insert Into Boss (BossName, BossId, ZoneId) VALUES (@bossName, @bossId, @zoneId)";
                List<string> parameterNames = new List<string> { "@bossName", "@bossId", "@zoneId"};
                ArrayList parameterValues = new ArrayList { boss.name, boss.id, zone.id };
                ExecuteQuery(cmdStr, parameterNames, parameterValues);
            }

        }

        public static List<int> GetItemIds()
        {
            List<int> intList = new List<int>();
            DataTable dt = ExecuteQuery("Select ItemId from Item");

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                intList.Add(dt.Rows[i].Field<int>("ItemId"));
            }

            return intList;
        }

        public static List<int> GetErrorIds(ErrorType errorType)
        {
            List<int> intList = new List<int>();
            DataTable dt;
            if (errorType == ErrorType.All)
            {
                dt = ExecuteQuery("Select Id From MigrationError");
            }
            else
            {
                string cmdStr = "Select Id From MigrationError Where Type = @errorType";
                List<string> parameterNames = new List<string> { "@errorType" };
                ArrayList parameterValues = new ArrayList  {errorType };
                dt = ExecuteQuery(cmdStr, parameterNames, parameterValues);
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                intList.Add(dt.Rows[i].Field<int>("Id"));
            }

            return intList;
        }

        public static void InsertError(ErrorType errorType, int id)
        {
            string cmdStr = "Insert Into MigrationError (Id, Type) VALUES (@id, @errorType)";
            List<string> parameterNames = new List<string> { "@id", "@errorType" };
            ArrayList parameterValues = new ArrayList { id, errorType };
            ExecuteQuery(cmdStr, parameterNames, parameterValues);
        }

        public static List<int> GetZoneIds()
        {
            DataTable dt = ExecuteQuery("Select ZoneId from Zone");
        }

        static DataTable ExecuteQuery(string query, List<string> parameterNames = null, ArrayList parameterValues = null)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand queryCommand = new SqlCommand(query, connection);
                if (parameterNames != null)
                {
                    for (int i = 0; i < parameterNames.Count(); i++)
                    {
                        queryCommand.Parameters.AddWithValue(parameterNames[i], parameterValues[i]);
                    }
                }
                SqlDataReader queryCommandReader = queryCommand.ExecuteReader();
                DataTable dataTable = new DataTable();
                dataTable.Load(queryCommandReader);
                connection.Close();
                return dataTable;
            }
        }
    }
}
