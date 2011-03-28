using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

/// <summary>
/// Summary description for Menus
/// </summary>
public static class Menus
{
    [Hook]
    public static dynamic get_module_admin_menu(dynamic data)
    {
        var menus = (IList<string>)data;
        menus.Add("Edit MenuTree|~/Pages/EditMenuTree");
        return data;
    }

}