﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Inspinia_MVC5_SeedProject.Models;

namespace Inspinia_MVC5_SeedProject.CodeTemplates
{
    public class CompanyController : Controller
    {
        private Entities db = new Entities();

        // GET: /Company/
        public ActionResult Index()
        {
            var companies = db.Companies.Include(c => c.AspNetUser).Include(c => c.City).Include(c => c.popularPlace);
            return View(companies.ToList());
        }

        // GET: /Company/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            ViewBag.companyId = company.Id;
            return View();
        }
        public ActionResult Ask()
        {
            return View();
        }
        public ActionResult Discussion(int id)
        {
            ViewBag.discussionId = id;
            return View();
        }
        // GET: /Company/Create
        public ActionResult Create()
        {
            Company company = new Company();
            return View(company);
        }
        public void SaveTags(Company ad)
        {
            string s = Request["tags"];
            string[] values = s.Split(',');
            Tag[] tags = new Tag[values.Length];
            CompanyTag[] qt = new CompanyTag[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = values[i].Trim();
                string ss = values[i];
                if (ss != "")
                {
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
                }
                else
                {
                    tags[i] = null;
                }
            }
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                string sb = e.ToString();
            }
            for (int i = 0; i < values.Length; i++)
            {
                if (tags[i] != null)
                {
                    qt[i] = new CompanyTag();
                    qt[i].companyId = ad.Id;
                    qt[i].tagId = tags[i].Id;
                    db.CompanyTags.Add(qt[i]);
                }
            }
        }
        // POST: /Company/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,title,shortabout,longabout,since,contactNo1,contactNo2,email,fblink,twlink,websitelink,owner,logoextension,category,createdBy,time,status,cityId,popularPlaceId,exectLocation")] Company company)
        {
            if (ModelState.IsValid)
            {
                if (Request.IsAuthenticated)
                {
                    company.createdBy = User.Identity.GetUserId();
                    company.time = DateTime.UtcNow;
                    company.status = "a";
                    company.category = "Services";
                    db.Companies.Add(company);
                    SaveTags(company);
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (DbEntityValidationException e)
                    {
                        string s = e.ToString();
                        List<string> errorMessages = new List<string>();
                        foreach (DbEntityValidationResult validationResult in e.EntityValidationErrors)
                        {
                            string entityName = validationResult.Entry.Entity.GetType().Name;
                            foreach (DbValidationError error in validationResult.ValidationErrors)
                            {
                                errorMessages.Add(entityName + "." + error.PropertyName + ": " + error.ErrorMessage);
                            }
                        }
                    }
                    return RedirectToAction("Details", new {id = company.Id });
                }
                
            }

            return View(company);
        }

        // GET: /Company/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            ViewBag.createdBy = new SelectList(db.AspNetUsers, "Id", "Email", company.createdBy);
            ViewBag.cityId = new SelectList(db.Cities, "Id", "addedBy", company.cityId);
            ViewBag.popularPlaceId = new SelectList(db.popularPlaces, "Id", "name", company.popularPlaceId);
            return View(company);
        }

        // POST: /Company/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,title,shortabout,longabout,since,contactNo1,contactNo2,email,fblink,twlink,websitelink,owner,logoextension,category,createdBy,time,status,cityId,popularPlaceId,exectLocation")] Company company)
        {
            if (ModelState.IsValid)
            {
                db.Entry(company).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.createdBy = new SelectList(db.AspNetUsers, "Id", "Email", company.createdBy);
            ViewBag.cityId = new SelectList(db.Cities, "Id", "addedBy", company.cityId);
            ViewBag.popularPlaceId = new SelectList(db.popularPlaces, "Id", "name", company.popularPlaceId);
            return View(company);
        }

        // GET: /Company/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // POST: /Company/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Company company = db.Companies.Find(id);
            db.Companies.Remove(company);
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