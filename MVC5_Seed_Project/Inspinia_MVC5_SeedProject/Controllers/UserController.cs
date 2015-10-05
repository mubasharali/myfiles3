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

namespace Inspinia_MVC5_SeedProject.Controllers
{
    public class UserController : ApiController
    {
        private Entities db = new Entities();

        // GET api/User
        public IQueryable<AspNetUser> GetAspNetUsers()
        {
            return db.AspNetUsers;
        }
        [HttpPost]
        public async Task<IHttpActionResult> CheckEmail(string email){
            var ret =await db.AspNetUsers.FirstOrDefaultAsync(x => x.UserName.Equals(email));
            if (ret == null)
            {
                return Ok("NewUser");
            }
            return Ok(ret.Email);
        }
        [HttpPost]
        public async Task<IHttpActionResult> CheckLoginUserPassword(string email,string password)
        {
            ApplicationDbContext ctx = new ApplicationDbContext();
            UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(ctx);
            UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(store);
            password = UserManager.PasswordHasher.HashPassword(password);  
            //var ret = await db.AspNetUsers.FirstOrDefaultAsync(x => x.UserName.Equals(email) && x.PasswordHash.Equals(password));
            var user = await UserManager.FindAsync(email,password);
            if (user != null)
            {
                
    //            await HttpContext.GetOwinContext()
    //.Get<ApplicationSignInManager>().SignInAsync(user, true, false); 
              //  await  SignInAsync(user, true,true); 
                return Ok("Incorrect");
            }
            return Ok("Ok");
        }
        
        [HttpPost]
        public async Task<IHttpActionResult> SaveProfilePic()
        {
            //save extension in database.
            return Ok();
        }
        public async Task<IHttpActionResult> SaveAd(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                SaveAd save =  db.SaveAds.FirstOrDefault(x=>x.savedBy.Equals(userId) && x.adId.Equals(id));
                if(save != null){
                    db.SaveAds.Remove(save);
                    await db.SaveChangesAsync();
                    return Ok("Deleted");
                }
                SaveAd ad = new SaveAd();
                ad.time = DateTime.UtcNow;
                ad.adId = id;
                ad.savedBy = User.Identity.GetUserId();
                db.SaveAds.Add(ad);
                await db.SaveChangesAsync();
                return Ok("Saved");
            }
            return BadRequest("Not login");
        }
        //public async Task<IHttpActionResult> SendMessageTo(string id)
        //{}

        // GET api/User/5
        [ResponseType(typeof(AspNetUser))]
        public async Task<IHttpActionResult> GetUser(string id)
        {
            AspNetUser aspnetuser = await db.AspNetUsers.FindAsync(id);
           // AspNetUser aspnetuser = db.AspNetUsers.FirstOrDefault(id);
            if (aspnetuser == null)
            {
                return NotFound();
            }
            string loginUserId = "";
            if (User.Identity.IsAuthenticated)
            {
                loginUserId = User.Identity.GetUserId();
            }
            var user = (from u in db.AspNetUsers
                       where u.Id.Equals(id)
                       select new
                       {
                           UserName = u.UserName,
                           Email = u.Email,
                           Id = u.Id,
                           dateOfBirth = u.dateOfBirth,
                           gender = u.gender,
                           hideEmail = u.hideEmail,
                           hidePhoneNumber = u.hidePhoneNumber,
                           hideDateOfBirth = u.hideDateOfBirth,
                           phoneNumber = u.PhoneNumber,
                           shortAbout = u.shortAbout,
                           longAbout = u.longAbout,
                           dpExtension = u.dpExtension,
                           lastSeen = Membership.UserIsOnlineTimeWindow,
                           reputation = u.reputation,
                           since = u.since,
                           city = u.city,
                           
                           loginUserId = loginUserId,
                       }).FirstOrDefault();
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
        //public async Task<IHttpActionResult> IsUserOnline(string id)
        //{
        //    var ret = "false";
        //    if (Membership.GetUser(id).IsOnline)
        //    {
        //        ret = "true";
        //        return Ok(ret);
        //    }
        //    return Ok(ret);
        //}
         public async Task<IHttpActionResult> UpdateProfile(AspNetUser user)
         {
             if (User.Identity.IsAuthenticated)
             {
                 if (User.Identity.GetUserId() == user.Id)
                 {
                     if (!ModelState.IsValid)
                     {
                         return BadRequest();
                     }
                     db.Entry(user).State = EntityState.Modified;

                     try
                     {
                         await db.SaveChangesAsync();
                     }
                     catch (DbUpdateConcurrencyException)
                     {
                         if (!AspNetUserExists(user.Id))
                         {
                             return NotFound();
                         }
                         else
                         {
                             throw;
                         }
                     }

                     return StatusCode(HttpStatusCode.NoContent);
                 }
             }
             return BadRequest();
        }
        // PUT api/User/5
        public async Task<IHttpActionResult> PutAspNetUser(string id, AspNetUser aspnetuser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != aspnetuser.Id)
            {
                return BadRequest();
            }

            db.Entry(aspnetuser).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AspNetUserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST api/User
        [ResponseType(typeof(AspNetUser))]
        public async Task<IHttpActionResult> PostAspNetUser(AspNetUser aspnetuser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.AspNetUsers.Add(aspnetuser);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AspNetUserExists(aspnetuser.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = aspnetuser.Id }, aspnetuser);
        }

        // DELETE api/User/5
        [ResponseType(typeof(AspNetUser))]
        public async Task<IHttpActionResult> DeleteAspNetUser(string id)
        {
            AspNetUser aspnetuser = await db.AspNetUsers.FindAsync(id);
            if (aspnetuser == null)
            {
                return NotFound();
            }

            db.AspNetUsers.Remove(aspnetuser);
            await db.SaveChangesAsync();

            return Ok(aspnetuser);
        }

        protected override void Dispose(bool disposing)
       {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AspNetUserExists(string id)
        {
            return db.AspNetUsers.Count(e => e.Id == id) > 0;
        }
    }
}