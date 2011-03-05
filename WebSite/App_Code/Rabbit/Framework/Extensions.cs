using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Dynamic;
using WebMatrix.Data;
using System.Collections.Specialized;
using System.IO;
using System.Collections;
using System.Text;
using System.Web.Script.Serialization;

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

    public static void EnsureProperty(this IDictionary<string, object> obj, string propertyName, object propertyValue)
    {
        if (obj == null) return;
        if (!((IDictionary<string, object>)obj).ContainsKey(propertyName))
        {
            ((IDictionary<string, object>)obj)[propertyName] = propertyValue;
        }
    }

    public static string ToJson(this ExpandoObject expando)
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        StringBuilder sb = new StringBuilder(); 

        List<string> contents = new List<string>(); 
        var d = expando as IDictionary<string, object>; 
        
        sb.Append("{");
        foreach (KeyValuePair<string, object> kvp in d)
        {
            if (kvp.Value is ExpandoObject)
            {
                contents.Add(String.Format("\"{0}\":{1}", kvp.Key, ((ExpandoObject) kvp.Value).ToJson()));
            }
            else
            {
                contents.Add(String.Format("\"{0}\":{1}", kvp.Key, serializer.Serialize(kvp.Value)));
            }
        } 

        sb.Append(String.Join(",", contents.ToArray())); 
        sb.Append("}"); 
        return sb.ToString();
    }


    public static ExpandoObject ToDynamic(this string text)
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        var dictionary = serializer.Deserialize<IDictionary<string, object>>(text);
        return dictionary.ToExpando();
    }

    /// <summary>
    /// http://coderjournal.com/2010/07/turning-json-into-a-expandoobject/
    /// </summary>
    /// <param name="dictionary"></param>
    /// <returns></returns>
    public static ExpandoObject ToExpando(this IDictionary<string, object> dictionary)
    {
        var expando = new ExpandoObject();
        var expandoDic = (IDictionary<string, object>)expando;
        foreach (var item in dictionary)
        {
            bool alreadyProcessed = false; 
            if (item.Value is IDictionary<string, object>)
            {
                expandoDic.Add(item.Key, ToExpando((IDictionary<string, object>)item.Value));
                alreadyProcessed = true;
            }
            else if (item.Value is ICollection)
            {
                var itemList = new List<object>();
                foreach (var item2 in (ICollection)item.Value)
                {
                    if (item2 is IDictionary<string, object>)
                        itemList.Add(ToExpando((IDictionary<string, object>)item2));
                    else
                    {
                        //itemList.Add(ToExpando(new Dictionary<string, object> { { "Unknown", item2 } }));
                        expandoDic.Add(item.Key, item.Value);
                        alreadyProcessed = true;
                        break;
                    }
                }
                if (itemList.Count > 0)
                {
                    expandoDic.Add(item.Key, itemList.ToArray()); 
                    alreadyProcessed = true;
                }
            }
            else if (item.Value is DateTime)
            {
                expandoDic.Add(item.Key, ((DateTime)item.Value).ToLocalTime());
                alreadyProcessed = true;
            }
            if (!alreadyProcessed) expandoDic.Add(item);
        } 
        return expando;
    }
}