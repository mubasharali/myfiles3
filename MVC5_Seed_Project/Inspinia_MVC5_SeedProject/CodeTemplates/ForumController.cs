using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using Microsoft.AspNet.Identity;
using Inspinia_MVC5_SeedProject.Models;

namespace Inspinia_MVC5_SeedProject.CodeTemplates
{
    public class ForumController : Controller
    {
        private Entities db = new Entities();

        // GET: /Forum/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Mobiles()
        {
            ViewBag.type = "mobiles";
            return View("Index");
        }
        // GET: /Forum/Details/5
        public ActionResult Details(int? id)
        {
            ViewBag.questionId = id;
            return View();
        }

        // GET: /Forum/Create
        public ActionResult Create()
        {
            ViewBag.postedBy = new SelectList(db.AspNetUsers, "Id", "Email");
            return View();
        }

        // POST: /Forum/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include="Id,category,postedBy,time,title,description")] Question question)
        {
            if (User.Identity.IsAuthenticated) { 
            if (ModelState.IsValid)
            {

                question.time = DateTime.UtcNow;
                question.postedBy = User.Identity.GetUserId();
                db.Questions.Add(question);

                string s = Request["tags"];
                string[] values = s.Split(',');
                Tag []tags = new Tag[values.Length];
                QuestionTag []qt = new QuestionTag[values.Length];
                //int count = 0;
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = values[i].Trim();
                    string ss = values[i];
                    var data = db.Tags.FirstOrDefault(x => x.name.Equals(ss, StringComparison.OrdinalIgnoreCase));

                    tags[i] = new Tag();
                    if (data != null)
                    {
                        tags[i].Id = data.Id;
                    }
                    else
                    {
                        tags[i].name = values[i];
                        tags[i].time = DateTime.UtcNow;
                        tags[i].createdBy = User.Identity.GetUserId();
                        db.Tags.Add(tags[i]);
                    }
                     

                    //MailMessage mail = new MailMessage();
                    //mail.From = new System.Net.Mail.MailAddress("m.irfanwatoo@gmail.com");

                    //// The important part -- configuring the SMTP client
                    //SmtpClient smtp = new SmtpClient();
                    //smtp.Port = 587;   // [1] You can try with 465 also, I always used 587 and got success
                    //smtp.EnableSsl = true;
                    //smtp.DeliveryMethod = SmtpDeliveryMethod.Network; // [2] Added this
                    //smtp.UseDefaultCredentials = false; // [3] Changed this
                    //smtp.Credentials = new NetworkCredential("m.irfanwatoo@gmail.com","birthdaywish");  // [4] Added this. Note, first parameter is NOT string.
                    //smtp.Host = "smtp.gmail.com";

                    ////recipient address
                    //mail.To.Add(new MailAddress("irfanyusanif420@gmail.com"));

                    ////Formatted mail body
                    //mail.IsBodyHtml = true;
                    //string st = "Test";

                    //mail.Body = st;
                    //smtp.Send(mail);

                }


                db.SaveChanges();
                

                try { 
                db.SaveChanges();
                }
                catch (Exception e)
                {
                    string sb = e.ToString();
                }
                for (int i = 0; i < values.Length; i++)
                {
                    qt[i] = new QuestionTag();
                    qt[i].questionId = question.Id;
                    qt[i].tagId = tags[i].Id;
                    db.QuestionTags.Add(qt[i]);
                }

                db.SaveChanges();
                return RedirectToAction("Details", new { id=question.Id});
            }

            ViewBag.postedBy = new SelectList(db.AspNetUsers, "Id", "Email", question.postedBy);
            return View(question);
            }
            return View(question);
        }

        // GET: /Forum/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            ViewBag.postedBy = new SelectList(db.AspNetUsers, "Id", "Email", question.postedBy);
            return View(question);
        }

        // POST: /Forum/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,category,subCategory,postedBy,time,title,description")] Question question)
        {
            if (ModelState.IsValid)
            {
                db.Entry(question).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.postedBy = new SelectList(db.AspNetUsers, "Id", "Email", question.postedBy);
            return View(question);
        }

        // GET: /Forum/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        // POST: /Forum/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Question question = db.Questions.Find(id);
            db.Questions.Remove(question);
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
