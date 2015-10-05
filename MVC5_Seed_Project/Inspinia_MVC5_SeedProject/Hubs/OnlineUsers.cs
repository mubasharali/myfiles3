using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Collections.Concurrent; // for userlist
using Microsoft.AspNet.Identity;
namespace Inspinia_MVC5_SeedProject.Hubs
{
    public class OnlineUsers : Hub
    {
        public static ConcurrentDictionary<string, string> users = new ConcurrentDictionary<string, string>();
        public override System.Threading.Tasks.Task OnConnected()
        {
            string userId = "visitor";
            if (Context.User.Identity.IsAuthenticated)
            {
                userId = Context.User.Identity.GetUserId();
            }
            string abc;
            var data = users.TryGetValue(Context.ConnectionId, out abc); // its not working
            if (!data)
            {
                users.TryAdd(Context.ConnectionId, userId);
            }
            Clients.All.showConnected(users);
            return base.OnConnected();
        }
        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            string abc;
            users.TryRemove(Context.ConnectionId, out abc);
            Clients.All.showConnected(users);
            return base.OnDisconnected(stopCalled);
        }
    }
}