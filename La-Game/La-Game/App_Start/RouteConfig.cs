using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace La_Game
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "ParticipantResultQuestionList",
                url: "participants/questionlistresult/{participantId}/{questionlistId}",
                defaults: new { controller = "Participants", action = "QuestionlistResult", participantId = UrlParameter.Optional, questionlistId = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "StudentTest", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
