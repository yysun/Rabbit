using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;
using System.Dynamic;
using System.Text.RegularExpressions;

/// <summary>
/// Summary description for Model
/// </summary>
public abstract class Model
{
    public dynamic Value { get; protected set; }

    protected void ValidateValue(Dictionary<string, string[]> rules)
    {
        var errors = new Dictionary<string, string>();

        foreach (var key in rules.Keys)
        {
            if (!((IDictionary<string, Object>)Value).Keys.Contains(key))
            {
                throw new InvalidOperationException(key + " is not a property and cannot be validated");
            }
            else
            {
                rules[key].ToList().ForEach(r => ValidateProperty(key, r, errors));
            }
        }

        Value.HasError = errors.Count() > 0;
        Value.Errors = errors;
    }

    private void ValidateProperty(string key, string rule, Dictionary<string, string> errors)
    {
        if (string.IsNullOrWhiteSpace(rule)) return;
        var message = "Please fix this field.";
        var target = "";

        if (rule.IndexOf("|") > 0)
        {
            var ss = rule.Split('|');
            rule = ss[0].ToLower().Trim();
            message = ss[1];
        }

        var rulename = rule;

        if (rule.IndexOf(":") > 0)
        {
            var ss = rule.Split(':');
            rule = ss[0].ToLower().Trim();
            target = ss[1];
        }

        var val = ((IDictionary<string, Object>)Value)[key];

        if (!RULES[rule](val, target))
        {
            errors.Add(key + ":" + rulename, string.Format(message, key));
        }
    }

    public static Dictionary<string, Func<object, string, bool>> RULES = 
        new Dictionary<string, Func<object, string, bool>>()
    {
        {"required",  (v, t) => { return v != null && !string.IsNullOrWhiteSpace(v.ToString()); }},
        {"regex",     (v, t) => { return v == null || Regex.IsMatch(v.ToString(), t); }},
        {"email",     (v, t) => { return v == null || Regex.IsMatch(v.ToString(), "\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*"); }},
        {"number",    (v, t) => { double d; return v == null || double.TryParse(v.ToString(), out d); }},
        {"digits",    (v, t) => { return v == null || Regex.IsMatch(v.ToString(), "\\d+"); }},
        {"date",      (v, t) => { DateTime d; return v == null || DateTime.TryParse(v.ToString(), out d); }},
        {"minlength", (v, t) => { return v == null || v.ToString().Length >= int.Parse(t); }},
        {"maxlength", (v, t) => { return v == null || v.ToString().Length <= int.Parse(t); }},
    };

}