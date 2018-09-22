using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using System.Text.RegularExpressions;

namespace WoWDataMigrate
{
    class Protractor
    {
        const string CHROMEDRIVERPATH = @"C:\Chromedriver";
        ChromeDriver driver = new ChromeDriver(CHROMEDRIVERPATH);
        string baseUrl = "https://www.wowhead.com/";

        void Execute()
        {
            driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(10);
        }
    }
}
