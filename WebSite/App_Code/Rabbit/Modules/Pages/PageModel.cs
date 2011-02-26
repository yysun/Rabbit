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

    public static PageModel List(dynamic data)
    {
        var model = new PageModel();
        var filepath = HttpContext.Current.Server.MapPath("~/App_Data/Pages");
        if (Directory.Exists(filepath))
        {
            data.List = Directory.EnumerateFiles(filepath, "*.*") //Sorting and Paging?
                           .Select(f =>
                           {
                               dynamic item = new ExpandoObject();
                               item.Id = Path.GetFileNameWithoutExtension(f);
                               item.Title = File.ReadLines(f).FirstOrDefault();
                               return item;
                           });
            
            model.Value = data; //?
        }
        return model;
    }

    public static PageModel Load(dynamic item)
    {
        var model = new PageModel();
        var filename = HttpContext.Current.Server.MapPath("~/App_Data/Pages/" + item.Id as string);

        if (File.Exists(filename))
        {
            var lines = File.ReadAllLines(filename);
            if (lines.Length > 0)
            {
                item.Title = lines[0];
                if (lines.Length > 1)
                {
                    item.Content = string.Join("\r\n", lines, 1, lines.Length - 1);
                }
            }
        }

        model.Value = item;
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
            var filename = HttpContext.Current.Server.MapPath("~/App_Data/Pages/" + Value.Id as string);
            using (var file = new StreamWriter(filename))
            {
                file.WriteLine(Value.Title);
                file.WriteLine(Value.Content);
            }
        }
        return this;
    }
    
    public PageModel Create()
    {
        Validate();
        if (Value != null && !Value.HasError)
        {
            //Create
        }
        return this;
    }
    
    public PageModel Delete()
    {
        if (Value != null && !Value.HasError)
        {
            var filename = HttpContext.Current.Server.MapPath("~/App_Data/Pages/" + Value.Id as string);
            if (File.Exists(filename)) File.Delete(filename);
            Value = null;
        }
        return this;
    }   
}    
      