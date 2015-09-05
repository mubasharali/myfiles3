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
        public async Task<IHttpActionResult> SaveProfilePic()
        {
            //save extension in database.
            return Ok();
        }
        // GET api/User/5
        [ResponseType(typeof(AspNetUser))]
        public async Task<IHttpActionResult> GetUser(string id)
        {
            AspNetUser aspnetuser = await db.AspNetUsers.FindAsync(id);
            var user = from u in db.AspNetUsers
                       where u.Id.Equals(id)
                       select new
                       {
                           UserName = u.UserName,
                           Email = u.Email,
                           Id = u.Id
                       };
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
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