using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Inspinia_MVC5_SeedProject.Models;

namespace Inspinia_MVC5_SeedProject.CodeTemplates
{
    public class JobsController : Controller
    {
        private Entities db = new Entities();
        ElectronicsController electronicController = new ElectronicsController();
        // GET: /Jobs/
        public async Task<ActionResult> Index()
        {
            var ads = db.Ads.Include(a => a.AspNetUser).Include(a => a.AdsLocation).Include(a => a.MobileAd);
            return View(await ads.ToListAsync());
        }

        // GET: /Jobs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ad ad = await db.Ads.FindAsync(id);
            if (ad == null)
            {
                return HttpNotFound();
            }
            return View(ad);
        }

        // GET: /Jobs/Create
        public ActionResult Create()
        {
            if (Request.IsAuthenticated)
            {
                Ad ad = new Ad();
                return View(ad);
            }
            return RedirectToAction("Index", "Home");
        }

        // POST: /Jobs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="Id,category,postedBy,title,description,time,price,isnegotiable")] Ad ad)
        {
            if (Request.IsAuthenticated)
            {
                if (ModelState.IsValid)
                {
                   await SaveAd(ad);
                    electronicController.SaveTags(Request["tags"],ref ad);
                   await SaveSkills(Request["skills"],  ad);
                    electronicController.PostAdByCompanyPage(ad.Id);
                    electronicController.MyAdLocation(Request["city"], Request["popularPlace"], Request["exectLocation"], ref ad, "Save");
                   await db.SaveChangesAsync();

                    return RedirectToAction("Details","Electronics", new { id = ad.Id, title = ad.title });
                }
            }
            return View(ad);
        }
        public async Task<object> SaveSkills(string s,Ad ad, bool update = false)
        {
            if (update)
            {
                var adid = ad.Id;
                var adtags = db.JobSkills.Where(x => x.adId.Equals(adid)).ToList();
                foreach (var cc in adtags)
                {
                    db.JobSkills.Remove(cc);
                }
                await db.SaveChangesAsync();
            }
            if(s == "" || s == null)
            {
                return true;
            }
            string[] values = s.Split(',');
            Tag[] tags = new Tag[values.Length];
            JobSkill[] qt = new JobSkill[values.Length];
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
                        tags[i].createdBy = System.Web.HttpContext.Current.User.Identity.GetUserId();
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
                    qt[i] = new JobSkill();
                    qt[i].adId = ad.Id;
                    qt[i].tagId = tags[i].Id;
                    db.JobSkills.Add(qt[i]);
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
            return true;
        }
        public async Task<bool> SaveAd(Ad ad,bool update = false)
        {
            JobAd jobAd = new JobAd();
            var seats = System.Web.HttpContext.Current.Request["seats"];
            if ( seats != null && seats != "")
            {
                jobAd.seats = int.Parse(seats);
            } 
            jobAd.qualification = System.Web.HttpContext.Current.Request["qualification"];
            jobAd.exprience = System.Web.HttpContext.Current.Request["exprience"];
            if(System.Web.HttpContext.Current.Request["salary"] != null && System.Web.HttpContext.Current.Request["salary"] != "")
            {
                ad.price = int.Parse(System.Web.HttpContext.Current.Request["salary"]);
            }
            ad.isnegotiable = System.Web.HttpContext.Current.Request["gender"];
            var skills = System.Web.HttpContext.Current.Request["skills"];
            jobAd.careerLevel = System.Web.HttpContext.Current.Request["careerLevel"];
            var lastDateToApply = System.Web.HttpContext.Current.Request["lastDateToApply"];
            if (lastDateToApply != null && lastDateToApply != "") 
            {
                jobAd.lastDateToApply = DateTime.Parse(lastDateToApply);
            }
            ad.condition = System.Web.HttpContext.Current.Request["shift"];
            jobAd.salaryType = System.Web.HttpContext.Current.Request["salaryType"];
            

            ad.description = System.Web.HttpUtility.HtmlEncode(ad.description);
            ad.postedBy = System.Web.HttpContext.Current.User.Identity.GetUserId();



            if (!update)
            {
                ad.category = System.Web.HttpContext.Current.Request["category"];
                ad.subcategory = System.Web.HttpContext.Current.Request["subcategory"];
                ad.time = DateTime.UtcNow;

                db.Ads.Add(ad);
                await db.SaveChangesAsync();
                jobAd.adId = ad.Id;
                db.JobAds.Add(jobAd);
                await db.SaveChangesAsync();
            }
            else if (update)
            {
                ad.time = DateTime.Parse(System.Web.HttpContext.Current.Request["time"]);
                ad.category = System.Web.HttpContext.Current.Request["category"];
                ad.subcategory = System.Web.HttpContext.Current.Request["subcategory"];
                db.Entry(ad).State = EntityState.Modified;
                await db.SaveChangesAsync();
                db.Entry(jobAd).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            

            

            return true;
        }
        // GET: /Jobs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ad ad = await db.Ads.FindAsync(id);
            if (ad == null)
            {
                return HttpNotFound();
            }
            ViewBag.postedBy = new SelectList(db.AspNetUsers, "Id", "Email", ad.postedBy);
            ViewBag.Id = new SelectList(db.AdsLocations, "Id", "cityId", ad.Id);
            ViewBag.Id = new SelectList(db.MobileAds, "Id", "color", ad.Id);
            return View(ad);
        }

        // POST: /Jobs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="Id,category,postedBy,title,description,time,price,isnegotiable")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ad).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.postedBy = new SelectList(db.AspNetUsers, "Id", "Email", ad.postedBy);
            ViewBag.Id = new SelectList(db.AdsLocations, "Id", "cityId", ad.Id);
            ViewBag.Id = new SelectList(db.MobileAds, "Id", "color", ad.Id);
            return View(ad);
        }

        // GET: /Jobs/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ad ad = await db.Ads.FindAsync(id);
            if (ad == null)
            {
                return HttpNotFound();
            }
            return View(ad);
        }

        // POST: /Jobs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Ad ad = await db.Ads.FindAsync(id);
            db.Ads.Remove(ad);
            await db.SaveChangesAsync();
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
