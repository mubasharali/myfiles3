using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Collections.Concurrent; // for userlist
using Microsoft.AspNet.Identity;
using Inspinia_MVC5_SeedProject.Models;
namespace Inspinia_MVC5_SeedProject.Hubs
{
    public class User
    {
        public string id;
        public string name;
        public string dpExtension;
    }
    public class OnlineUsers : Hub
    {
        private Entities db = new Entities();
        public static ConcurrentDictionary<string, User> users = new ConcurrentDictionary<string, User>();
        public override System.Threading.Tasks.Task OnConnected()
        {
            User u = new User();
            u.id = "visitor";
            u.name = "Visitor";
            u.dpExtension = "";
            if (Context.User.Identity.IsAuthenticated)
            {
                u.id = Context.User.Identity.GetUserId();
                var da = db.AspNetUsers.Find(u.id);
                u.name = da.Email;
                u.dpExtension = da.dpExtension;
            }
            User abc;
            var data = users.TryGetValue(Context.ConnectionId, out abc); // its not working
            if (!data)
            {

                users.TryAdd(Context.ConnectionId, u);
            }
            Clients.All.showConnected(users);
            return base.OnConnected();
        }
        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            User abc;
            users.TryRemove(Context.ConnectionId, out abc);
            Clients.All.showConnected(users);
            return base.OnDisconnected(stopCalled);
        }
    }
}