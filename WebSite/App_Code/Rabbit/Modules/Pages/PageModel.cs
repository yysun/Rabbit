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
    public PageModel()
    {
        Value = new ExpandoObject();
        Repository = new Repository("Pages");
    }

    public PageModel List(int pageNo, int pageSize, Func<ExpandoObject, bool> filter = null)
    {
        Value.PageNo = pageNo;
        Value.PageSize = pageSize;
        IEnumerable<ExpandoObject> list = Repository.List(filter);

        //if (filter != null) list = list.Where(p => filter(p));
        //if (sort != null) sort = list.OrderBy((p=>sort(p));
        
        Value.List = list == null ? null :
            list.Skip((pageNo - 1) * pageSize).Take(pageSize);

        return this;
    }

    public PageModel Load(dynamic item)
    {
        Value = Repository.Load(item.Id as string);
        return this;
    }

    public PageModel Validate()
    {
        Assert.IsTrue(Value != null);

        var rules = new Dictionary<string, string[]>
        {
            {"Id", new string[]{"required", "minlength:2", "maxlength:140"}},
            {"Title", new string[]{"required", "minlength:2", "maxlength:140"}},
        };

        this.ValidateValue(rules);
        return this;
    }

    public PageModel Update(dynamic item)
    {
        Assert.IsTrue(item != null);
        Value = item;
        Validate();
        if (!HasError) Repository.Save(Value.Id as string, Value);
        return this;
    }

    public PageModel SaveAs(dynamic item, string newId)
    {
        Assert.IsTrue(item != null);
        var oldId = item.Id as string;
        Value = item;
        Value.Id = newId;
        Validate();
        if (HasError) return this;

        if (oldId != newId)
        {
            if (Repository.Exists(newId))
            {
                Errors.Add("Id", string.Format("{0} exisits already.", Value.Id));
            }
            else
            {
                Repository.Delete(oldId);
            }
        }

        if (!HasError) Value.Id = Repository.Save(Value.Id as string, Value);
        return this;
    }

    public PageModel Create(dynamic item)
    {
        Assert.IsTrue(item != null);
        Value = item;
        Value.Id = Value.Title;
        Validate();
        if (HasError) return this;

        if (Repository.Exists(Value.Id))
        {
            Errors.Add("Title", string.Format("{0} exisits already.", Value.Title));
        }

        if (!HasError) Value.Id = Repository.Save(Value.Id, Value);

        return this;
    }


    public PageModel Delete(dynamic item)
    {
        Value = item;
        if (Value != null && Value.Id != null)
        {
            Repository.Delete(Value.Id as string);
            Value = null;
        }
        return this;
    }   
}    
      