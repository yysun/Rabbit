using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rabbit
{
    /// <summary>
    /// Summary description for Model
    /// </summary>
    public abstract class Model
    {
        public dynamic Repository { get; set; }

        public dynamic Value { get; protected set; }

        private Dictionary<string, object> errors;
        public Dictionary<string, object> Errors { get { return errors; } }
        public bool HasError { get { return errors.Count() > 0; } }


        public Model()
        {
            errors = new Dictionary<string, object>();
        }

        public Model(dynamic data)
            : this()
        {
            Value = data;
        }

        protected void ValidateValue(Dictionary<string, string[]> rules)
        {

            if (Value == null)
            {
                errors.Add("_self", "Object to be validated is null.");
            }
            else
            {
                foreach (var key in rules.Keys)
                {
                    if (!((IDictionary<string, Object>)Value).Keys.Contains(key))
                    {
                        throw new InvalidOperationException(key + " is not a property and cannot be validated");
                    }
                    else
                    {
                        rules[key].ToList().ForEach(r => ValidateProperty(key, r));
                    }
                }
            }
        }

        protected void ValidateProperty(string key, string rule)
        {
            if (string.IsNullOrWhiteSpace(rule)) return;
            var message = rule;
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

            if (!RULES.ContainsKey(rule)) throw new ArgumentOutOfRangeException(rule + ": is not a valid validation rule.");

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
            {"email",     (v, t) => { return v == null || Regex.IsMatch(v.ToString(), "^\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*$"); }},
            {"number",    (v, t) => { double d; return v == null || double.TryParse(v.ToString(), out d); }},
            {"digits",    (v, t) => { return v == null || Regex.IsMatch(v.ToString(), "^\\d+$"); }},
            {"date",      (v, t) => { DateTime d; return v == null || DateTime.TryParse(v.ToString(), out d); }},
            {"minlength", (v, t) => { return v == null || v.ToString().Length >= int.Parse(t); }},
            {"maxlength", (v, t) => { return v == null || v.ToString().Length <= int.Parse(t); }},
        };
    }
}