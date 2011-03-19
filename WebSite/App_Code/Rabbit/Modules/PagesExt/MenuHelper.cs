using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages.Html;
using System.Text;

public static class MenuHelper
{
    public static string Menu(this HtmlHelper helper)
    {
        var sb = new StringBuilder();

        // Create opening unordered list tag
        sb.Append("<ul class='menu'>");

        // Render each top level node
        var topLevelNodes = SiteMap.RootNode.ChildNodes;
        foreach (SiteMapNode node in topLevelNodes)
        {
            if (SiteMap.CurrentNode == node)
                sb.AppendLine("<li class='selectedMenuItem'>");
            else
                sb.AppendLine("<li>");

            sb.AppendFormat("<a href='{0}'>{1}</a>", node.Url, helper.Encode(node.Title));
            sb.AppendLine("</li>");
        }

        // Close unordered list tag
        sb.Append("</ul>");

        return sb.ToString();
    }

}
