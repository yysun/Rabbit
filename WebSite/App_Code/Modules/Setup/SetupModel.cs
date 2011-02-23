using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Dynamic;
using System.IO;

/// <summary>
/// Summary description for SetupModel
/// </summary>
public class SetupModel : Model
{
	private SetupModel()
	{
	}

    public static SetupModel Load()
    {
        var filename = HttpContext.Current.Server.MapPath("~/App_Data/Site.txt");
        var lines = File.ReadAllLines(filename).Where(s => !s.StartsWith("#")).ToList();

        //TODO: load from file/db
        dynamic value = new ExpandoObject();
        lines.ForEach(s =>
        {
            if (s.IndexOf(":") > 0)
            {
                var ss = s.Split(':');
                ((IDictionary<string, object>)value)[ss[0]] = ss[1];
            }
        });
        return new SetupModel { Value = value };
    }

}