using System.Web;
using System.Web.Mvc;

namespace La_Game
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            // If you want access without logging in for testing purposes, comment the filter or use [AllowAnonymous] on things you need to test.
            filters.Add(new AuthorizeAttribute());
        }
    }
}
