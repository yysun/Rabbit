using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Dynamic;
using System.IO;
using System.Collections.Specialized;


/// <summary>
/// Summary description for PageModel
/// </summary>
public class PageModel : Model
{
   
    public dynamic Repository { get; set; }

    public PageModel()
    {
        Value = new ExpandoObject();
        Repository = new Repository("Pages");
    }

    public PageModel(dynamic data) : this()
    {
        Value = data;
    }

    public PageModel List(dynamic data) //Filtering, Sorting and Paging?
    {
        var model = new PageModel();
        data.List = Repository.List();
        model.Value = data;
        return model;
    }

    public PageModel Load(dynamic item)
    {
        var model = new PageModel();
        dynamic value = Repository.Load(item.Id as string) ?? item;
        model.Value = value;
        return model;
    }

    public PageModel Validate()
    {
        //generate safe file name
        if (string.IsNullOrWhiteSpace(Value.Id) & Value.Title != null)
        {
            string id = Value.Title;
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                id = id.Replace(c, '-');
            }
            foreach (char c in @"~`!@#$%^&+=,;""".ToCharArray())
            {
                id = id.Replace(c, '-');
            }
            Value.Id = id;
        }

        var rules = new Dictionary<string, string[]>
        {
            {"Id", new string[]{"required", "minlength:2", "maxlength:140"}},
            {"Title", new string[]{"required", "minlength:2", "maxlength:140"}},
        };

        this.ValidateValue(rules);
        return this;
    }
    
    public PageModel Save()
    {
        Validate();
        if (Value != null && !Value.HasError)
        {
            ((IDictionary<string, object>)Value).Remove("HasError");
            ((IDictionary<string, object>)Value).Remove("Errors");

            Repository.Save(Value.Id as string, Value);
        }
        return this;
    }
    
    //public PageModel Create()
    //{
    //    Validate();
    //    if (Value != null && !Value.HasError)
    //    {
    //        //Create
    //    }
    //    return this;
    //}
    
    public PageModel Delete()
    {
        if (Value != null && Value.Id != null)
        {
            Repository.Delete(Value.Id as string);
            Value = null;
        }
        return this;
    }   
}    
      