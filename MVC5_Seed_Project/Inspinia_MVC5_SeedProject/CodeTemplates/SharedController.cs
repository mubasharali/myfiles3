using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inspinia_MVC5_SeedProject.Models;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
namespace Inspinia_MVC5_SeedProject.CodeTemplates
{
    public class SharedController : Controller
    {
        private Entities db = new Entities();
        
        public string GetUserEmail()
        {
            if (Request.IsAuthenticated)
            {
                var user =  db.AspNetUsers.Find(User.Identity.GetUserId());
                return user.Email;
            }
            return "visitor";
        }
    }
}
