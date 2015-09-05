using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
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
            var questions = db.Questions.Include(q => q.AspNetUser);
            return View(questions.ToList());
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
        public ActionResult Create([Bind(Include="Id,category,subCategory,postedBy,time,title,description")] Question question)
        {
            if (ModelState.IsValid)
            {
                question.time = DateTime.UtcNow;
                question.postedBy = User.Identity.GetUserId();
                db.Questions.Add(question);
                db.SaveChanges();
                return RedirectToAction("Details", new { id=question.Id});
            }

            ViewBag.postedBy = new SelectList(db.AspNetUsers, "Id", "Email", question.postedBy);
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
