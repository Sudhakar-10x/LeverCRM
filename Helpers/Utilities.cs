using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace _10xErp.Helpers
{
    public static class Utilities
    {
        public static void SetResultMessage(string contents)
        {
            using (StreamWriter TextWriter = new StreamWriter(ConfigurationManager.AppSettings["LogFile"] + "\\" + DateTime.Now.ToString("yyyyMMdd") + "_Log.txt", true))
            {
                TextWriter.WriteLine(contents);
            }

        }


    }
}