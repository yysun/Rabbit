using System.Web.WebPages;

public static class Mvc_
{
    public static void Run(this WebPage page)
    {
        // run action
        // render view

        page.Page.Model = "MVC";
        page.Write(page.RenderPage("~/Test_Mvc_View.cshtml", page.Page.Model));
    }

	public static void Run_Mvc(this WebPage webpage)
	{
        Run(webpage);
	}
}