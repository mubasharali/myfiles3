using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace Inspinia_MVC5_SeedProject.Hubs
{
    public class OnlineUsers : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }
    }
}