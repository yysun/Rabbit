using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Dynamic;
using WebMatrix.Data;
using System.Collections.Specialized;

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
            ((IDictionary<string, Object>) obj) [col] = record[col];
        }
        return obj;
    }

    public static dynamic ToDynamic(this NameValueCollection form)
    {
        var obj = new ExpandoObject();
        foreach (var key in form.AllKeys)
        {
            ((IDictionary<string, Object>) obj)[key] = form[key];
        }
        return obj;
    }
}