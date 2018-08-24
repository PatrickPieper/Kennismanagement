using La_Game.Models;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace La_Game.Hubs
{
    public class LiveHub : Hub
    {
        public override Task OnConnected()
        {
            using (LaGameDBContext db = new LaGameDBContext())
            {
                var email = Context.User.Identity.Name;

                var member = db.Members.FirstOrDefault(x => x.email == email);

                if(member == null)
                {
                    throw new Exception();
                }

                Groups.Add(Context.ConnectionId, "Members");
            }

            


                return base.OnConnected();
        }
    }
}