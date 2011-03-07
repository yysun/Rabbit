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
public class PageExtModel : Model
{
    private PageExtModel()
    {
    }

    static PageExtModel()
    {
        Store = new ContentStore();
    }

    public static dynamic Store { get; set; }

    private static string ContentType = "Pages";

    public static PageExtModel List(dynamic data) //Filtering, Sorting and Paging?
    {
        var model = new PageExtModel();
        data.List = Store.LoadContent(ContentType);
        model.Value = data;
        return model;
    }

    public static PageExtModel Load(dynamic item)
    {
        var model = new PageExtModel();
        model.Value = Store.LoadContent(ContentType, item.Id as string) ?? item;
        return model;
    }

    public static PageExtModel New()
    {
        dynamic value = new ExpandoObject();
        return new PageExtModel { Value = value };
    }

    public static PageExtModel New(dynamic data)
    {
        return new PageExtModel { Value = data };
    }

    public PageExtModel Validate()
    {
        var rules = new Dictionary<string, string[]>
        {
            {"Id", new string[]{"required", "minlength:2", "maxlength:140"}},
            {"Title", new string[]{"required", "minlength:2", "maxlength:140"}},
        };

        this.ValidateValue(rules);
        return this;
    }

    public PageExtModel Save()
    {
        Validate();
        if (Value != null && !Value.HasError)
        {
            ((IDictionary<string, object>)Value).Remove("HasError");
            ((IDictionary<string, object>)Value).Remove("Errors");
            ContentStore.SaveContent(ContentType, Value.Id as string, Value);
        }
        return this;
    }

    public PageExtModel Create()
    {
        Validate();
        if (Value != null && !Value.HasError)
        {
            //Create
        }
        return this;
    }

    public PageExtModel Delete()
    {
        if (Value != null && Value.Id != null)
        {
            Store.DeleteContent(ContentType, Value.Id as string);
            Value = null;
        }
        return this;
    }   
}    
      