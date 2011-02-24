using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Dynamic;
using WebMatrix.Data;
using System.Collections.Specialized;
using System.IO;

/// <summary>
/// Summary description for Extensions
/// </summary>
public static class Extensions
{
    public static dynamic ToDynamic(this DynamicRecord record)
    {
        if (record == null) return null;

        var obj = new ExpandoObject();
        foreach (var col in record.Columns)
        {
            ((IDictionary<string, Object>)obj)[col] = record[col];
        }
        return obj;
    }

    public static dynamic ToDynamic(this NameValueCollection form)
    {
        var obj = new ExpandoObject();
        foreach (var key in form.AllKeys)
        {
            ((IDictionary<string, Object>)obj)[key] = form[key];
        }
        return obj;
    }

    public static dynamic ToDynamic(this IEnumerable<string> strings)
    {
        if (strings == null) return null;
        var obj = new ExpandoObject();
        strings.ToList().ForEach(s =>
        {
            var pos = s.IndexOf(":");
            if (pos > 0)
            {
                ((IDictionary<string, object>)obj)[s.Substring(0, pos)] = s.Substring(pos + 1);
            }
        });
        return obj;
    }

    public static void SaveToFile(this IDictionary<string, object> obj, string filename)
    {
        using (var file = new StreamWriter(filename))
        {
            foreach (var item in obj)
            {
                file.WriteLine(string.Format("{0}:{1}", item.Key, item.Value));
            }
        }
    }

    public static void EnsureProperty(this IDictionary<string, object> obj, string propertyName, object propertyValue)
    {
        if (!((IDictionary<string, object>)obj).ContainsKey(propertyName))
        {
            ((IDictionary<string, object>)obj)[propertyName] = propertyValue;
        }
    }
}