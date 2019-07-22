using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using System.Text.RegularExpressions;

namespace WoWDataMigrate
{
    static public class Protractor
    {
        const string CHROMEDRIVERPATH = @"C:\Chromedriver";
        const string baseUrl = "https://www.wowhead.com/";
        static ChromeOptions options = new ChromeOptions();
        static ChromeDriver driver = new ChromeDriver(CHROMEDRIVERPATH, options, TimeSpan.FromMinutes(8));

        static public void Teardown()
        {
            driver.Quit();            
        }

        static public List<int> GetItemIds (int bossId)
        {
            List<int> list = new List<int>();
            driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(15);
            driver.Url = baseUrl + "npc=" + bossId.ToString();
            for (int i = 0; i < driver.FindElementsByClassName(Page.item).Count(); i++)
            {
                list.Add(ParseUrlForId(driver.FindElementsByClassName(Page.item)[i].GetAttribute("href")));
            }
            driver.ResetInputState();
            return list;
        }

        static int ParseUrlForId(string url)
        {
            string itemId = "";
            for (int i = url.IndexOf('i'); i < url.Length; i++)
            {
                if (url[i] == '/' || url[i] == '&')
                {
                    break;
                }
                if (char.IsNumber(url[i]))
                {
                    itemId = itemId + url[i];
                }
            }
            return Int32.Parse(itemId);
        }
    }

    static public class Page
    {
        public const string item = "listview-cleartext";
        public const string searchBar = "q";
        public const string searchResult = "live-search-icon";
        public const string searchButton = "header-search-button";
        public const string searchButton2 = "zul-bar-user-item-share";
        public const string searchWindow = " q3";
    }

    
}
