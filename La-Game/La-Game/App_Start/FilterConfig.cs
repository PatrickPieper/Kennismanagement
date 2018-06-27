using La_Game.Models;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace La_Game
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            // Filters
            filters.Add(new HandleErrorAttribute());
            //filters.Add(new AuthorizeAttribute());
        }
    }
}

/// <summary>
/// Custom authorization attribute that checks if the user has admin rights.
/// </summary>
public class AuthorizeAdmin : AuthorizeAttribute
{
    // Database context
    private LaGameDBContext db = new LaGameDBContext();

    /// <summary>
    /// On authorization perform the check.
    /// </summary>
    /// <param name="context"> The authorizationcontext. </param>
    public override void OnAuthorization(AuthorizationContext context)
    {
        // Get the list of admins and the current user
        var adminList = db.Members.Where(a => a.isAdmin == 1).Select(e => e.email).ToList();
        var currentUser = HttpContext.Current.User.Identity.Name;

        // Check if the adminlist contains the current user
        if (!adminList.Contains(currentUser))
        {
            context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Authentication", action = "Forbidden" }));
        }
    }
}
