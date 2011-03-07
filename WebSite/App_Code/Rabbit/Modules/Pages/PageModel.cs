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
    private PageModel()
    {
    }

    private static string ContentType = "Pages";    
    
    static PageModel()
    {
        Store = new ContentStore();
    }
    public static dynamic Store { get; set; }

    public static PageModel List(dynamic data) //Filtering, Sorting and Paging?
    {
        var model = new PageModel();
        data.List = Store.LoadContent(ContentType);
        model.Value = data;
        return model;
    }

    public static PageModel Load(dynamic item)
    {
        var model = new PageModel();
        dynamic value = Store.LoadContent(ContentType, item.Id as string) ?? item;
        model.Value = value;
        return model;
    }

    public static PageModel New()
    {
        dynamic value = new ExpandoObject();
        return new PageModel { Value = value };
    }

    public static PageModel New(dynamic data)
    {
        return new PageModel { Value = data };
    }

    public PageModel Validate()
    {
        //generate safe file name
        if (string.IsNullOrWhiteSpace(Value.Id))
        {
            string id = Value.Title;
            foreach (char c in Path.GetInvalidFileNameChars())
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

            Store.SaveContent(ContentType, Value.Id as string, Value);
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
            Store.DeleteContent(ContentType, Value.Id as string);
            Value = null;
        }
        return this;
    }   
}    
      