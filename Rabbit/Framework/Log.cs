using System;
using System.IO;
using System.Web;

namespace Rabbit
{
    /// <summary>
    /// Summary description for Log
    /// </summary>
    public static class Log
    {
        public static bool Enabled { get; set; }
        public static void Write(string format, params object[] args)
        {

            if (!Enabled || !HttpContext.Current.IsDebuggingEnabled) return;

            var fileName = HttpContext.Current.Server.MapPath("~/App_Data/Rabbit/Log");
            using (StreamWriter sw = File.AppendText(fileName))
            {
                sw.WriteLine(DateTime.Now.ToString() + " " + string.Format(format, args));
            }
        }
    }
}