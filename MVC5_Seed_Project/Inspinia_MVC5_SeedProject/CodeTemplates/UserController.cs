using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Net.Http;
using Inspinia_MVC5_SeedProject.Models;

namespace Inspinia_MVC5_SeedProject.CodeTemplates
{
    public class UserController : Controller
    {
        private Entities db = new Entities();

        // GET: /User/
        public ActionResult Index(string id)
        {
            //return View(db.AspNetUsers.ToList());
            ViewBag.id = id;
            return View();
        }
        public ActionResult Profile(string id)
        {
            ViewBag.userId = id;
            return View();
        }
        public ActionResult saveProfilePic()
        {
            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase file = Request.Files[i];
                string extension = System.IO.Path.GetExtension(file.FileName);
                file.SaveAs(Server.MapPath(@"~\Images\Users\p" + User.Identity.GetUserId()  + extension));
                string id = User.Identity.GetUserId();
                var user = db.AspNetUsers.Find(id);
                user.dpExtension = extension;
                db.SaveChanges();
            }

            return RedirectToAction("../User/Profile", new { id = User.Identity.GetUserId() });
        }
        //public ActionResult saveProfilePic()
        //{
        //    if (Request.IsAuthenticated)
        //    {
        //        for (int i = 0; i < Request.Files.Count; i++)
        //        {
        //            HttpPostedFileBase file = Request.Files[i];
        //            string extension = System.IO.Path.GetExtension(file.FileName);
        //            HttpClient client = new HttpClient();
        //            var stringContent = new StringContent(extension.ToString());

        //         //   content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        //            client.BaseAddress = new Uri("http://localhost:59322/api/User/SaveProfilePic");
        //            HttpResponseMessage response = client.PostAsync("/api/User/SaveProfilePic", stringContent).Result;
        //            if (!response.IsSuccessStatusCode)
        //            {
        //                TempData["errorMessage"] = "Some error occured. Please try again";
        //            }
        //            file.SaveAs(Server.MapPath(@"\Images\userProfilePic" + User.Identity.GetUserId()));
        //        }
        //        return RedirectToAction("Index", User.Identity.GetUserId());
        //    }
        //    else
        //    {
        //        TempData["errorMessage"] = "You are not login";
        //        return RedirectToAction("Login","Account");
        //    }
        //}
        // GET: /User/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspnetuser = db.AspNetUsers.Find(id);
            if (aspnetuser == null)
            {
                return HttpNotFound();
            }
            return View(aspnetuser);
        }

        // GET: /User/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /User/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] AspNetUser aspnetuser)
        {
            if (ModelState.IsValid)
            {
                db.AspNetUsers.Add(aspnetuser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(aspnetuser);
        }

        // GET: /User/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspnetuser = db.AspNetUsers.Find(id);
            if (aspnetuser == null)
            {
                return HttpNotFound();
            }
            return View(aspnetuser);
        }

        // POST: /User/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] AspNetUser aspnetuser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aspnetuser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(aspnetuser);
        }

        // GET: /User/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspnetuser = db.AspNetUsers.Find(id);
            if (aspnetuser == null)
            {
                return HttpNotFound();
            }
            return View(aspnetuser);
        }

        // POST: /User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            AspNetUser aspnetuser = db.AspNetUsers.Find(id);
            db.AspNetUsers.Remove(aspnetuser);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
