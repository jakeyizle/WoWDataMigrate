using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoWDataMigrate
{
    public static class UI
    {
        public static string Text(string display)
        {
            Console.Clear();
            Console.WriteLine(display);
            return Console.ReadLine();
        }
    }
}
