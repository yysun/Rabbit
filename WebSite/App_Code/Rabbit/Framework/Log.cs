using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

/// <summary>
/// Summary description for Log
/// </summary>
public static class Log
{
    public static bool Enabled { get; set; }
	public static void Write(string format, params object[] args)
	{
//#if DEBUG
        if (!Enabled) return;

        var fileName = HttpContext.Current.Server.MapPath("~/App_Data/Rabbit/Log");
        using (StreamWriter sw = File.AppendText(fileName))
        {
            sw.WriteLine(DateTime.Now.ToString() + " " + string.Format(format, args));
        }   

//#endif
	}
}