using La_Game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace La_Game
{
    public class FilterConfig
    {
        //private static LaGameDBContext db = new LaGameDBContext();

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {

            filters.Add(new HandleErrorAttribute());

            // If you want access without logging in for testing purposes, comment the filter or use [AllowAnonymous] on things you need to test.
            filters.Add(new AuthorizeAttribute());
            
            /*
            AuthorizeAttribute authorizeAttribute = new AuthorizeAttribute();
            var members = db.Members.ToList().Where(m => m.isAdmin == 1);

            List<string> admins = new List<string>();
            foreach (Member member in members)
            {
                admins.Add(member.email);
            }

            authorizeAttribute.Users = String.Join(",", admins);
            filters.Add(authorizeAttribute);
            */
        }
    }
}
