//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
using Microsoft.AspNet.SignalR;
//using Microsoft.AspNet.Identity;
//using Inspinia_MVC5_SeedProject.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using System.Web;
using Microsoft.Owin.Security;
using System.Security.Claims;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using Inspinia_MVC5_SeedProject.Models;
namespace Inspinia_MVC5_SeedProject.Hubs
{
    public class ChatHub : Hub
    {
        Entities db = new Entities();
        public void AddMessage(Chat msg)
        {
            msg.time = DateTime.UtcNow;
            msg.sentFrom = Context.User.Identity.GetUserId();
            db.Chats.Add(msg);
            db.SaveChanges();
            var ret = db.Chats.FirstOrDefault(x => x.Id == msg.Id);
            //Clients.Group(msg.sentTo).loadNewMessage(ret);
           // Clients.Caller.loadNewMessage(ret);
            Clients.All.loadNewMessage(ret);
        }
        public void GetAlert()
        {
            Clients.All.showAlert();
        }
        public void Hello()
        {

            Clients.All.hello();
        }
    }
}