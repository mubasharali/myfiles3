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
    public class ElectronicsController : Controller
    {
       
        private Entities db = new Entities();
        //public string subdomainName
        //{
        //    get
        //    {
        //        string s = Request.Url.Host;
        //        var index = s.IndexOf(".");

        //        if (index < 0)
        //        {
        //            return null;
        //        }
        //        var sub = s.Split('.')[0];
        //        if (sub == "www" || sub == "localhsot")
        //        {
        //            return null;
        //        }
        //        return sub;
        //    }
        //}
        // GET: /Electronics/
        //public ActionResult Index()
        //{
        //    var ads = db.Ads.Include(a => a.AspNetUser);
        //    return View(ads);
        //}
        public ActionResult Index(string category, string subcategory, string lowcategory, int? id)
        {
            
            if (category == null)//mobiles
            {
                return View("Index");
            }
            if (subcategory == null)//htc
            {
                //if (category == "mobiles")
                //{
                    ViewBag.category = category;
                    ViewBag.subcategory = subcategory;
                    ViewBag.lowcategory = lowcategory;
                    ViewBag.title = null;
               // }
                return View("search");
            }
            if (lowcategory == null)//M8
            {
                ViewBag.category = category;
                ViewBag.subcategory = subcategory;
                ViewBag.lowcategory = lowcategory;
                ViewBag.title = null;
                return View("search");
            }
            if (id == null)
            {
                ViewBag.cateogry = category;
                ViewBag.subcateogry = subcategory;
                ViewBag.lowcategory = lowcategory;
                ViewBag.title = null;
                return View("search");
            }
            if (id != null)
            {
                Ad ad = db.Ads.FirstOrDefault(x => x.Id == id);
                if (ad == null)
                {
                    return HttpNotFound();
                }
                if (category == "mobiles") // remove this logic
                {
                    var mobileAds = ad.MobileAds.FirstOrDefault();
                    ViewBag.ad = mobileAds;
                    ViewBag.category = "mobiles";
                    ViewBag.adId = id;
                }
                return View("Details",ad);
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        // GET: /Electronics/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ad ad = db.Ads.Find(id);
            if (ad == null)
            {
                return HttpNotFound();
            }
            ViewBag.adId = id;
            return View();
        }
        public ActionResult tree()
        {
            return View();
        }
        // GET: /Electronics/Create
        public ActionResult Create()
        {
            ViewBag.postedBy = new SelectList(db.AspNetUsers, "Id", "Email");
            Ad ad = new Ad();
            return View(ad);
        }
        public ActionResult CreateLaptopAd()
        {
            Ad ad = new Ad();
            return View(ad);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateLaptopAd([Bind(Include = "Id,category,postedBy,title,description,time")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                if (Request.IsAuthenticated)
                {
                    LaptopAd mobileAd = new LaptopAd();

                    var isbiding = Request["bidingAllowed"];
                    if (isbiding == "fixedPrice")
                    {
                        var nn = Request["isNegotiable"];
                        if (nn == "on")
                        {
                            ad.isnegotiable = "y";
                        }
                        else
                        {
                            ad.isnegotiable = "n";
                        }
                    }
                    else if (isbiding == "allowBiding")
                    {
                        ad.isnegotiable = "b";
                    }
                    var condition = Request["condition"];
                    if (condition == "new")
                    {
                        mobileAd.condition = "n";
                    }
                    else if (condition == "unboxed")
                    {
                        mobileAd.condition = "b";
                    }
                    else
                    {
                        mobileAd.condition = "u";
                    }
                    mobileAd.color = Request["color"];
                    var pp = Request["price"];
                    pp = pp.Replace(",", string.Empty);
                    if (pp != null && pp != "")
                    {
                        //if (pp.Substring(pp.Length - 1) == ",")
                        //{
                        //    pp = pp.Remove(pp.Length - 1);
                        //}
                        ad.price = int.Parse(pp);
                    }
                    var company = Request["brand"];
                    var model = Request["model"];
                    ad.time = DateTime.UtcNow;
                    ad.description = System.Web.HttpUtility.HtmlEncode(ad.description);
                    ad.postedBy = User.Identity.GetUserId();
                    AspNetUser asp = db.AspNetUsers.FirstOrDefault(x => x.Id == ad.postedBy);

                    ad.category = "Electronics";
                    //var companyName = model.Split(' ')[0];
                    var allBrands = (db.LaptopBrands.Select(x => x.Id)).AsEnumerable(); //getBrands
                    bool isNewBrand = true;
                    foreach (var brand in allBrands)
                    {
                        if (brand == company)
                        {
                            isNewBrand = false;
                        }
                    }
                    if (isNewBrand)
                    {
                        LaptopBrand mob = new  LaptopBrand();
                        mob.Id = company;
                        db.LaptopBrands.Add(mob);
                        db.SaveChanges();

                        LaptopModel mod = new  LaptopModel();
                        mod.model = model;
                        mod.brand = company;
                        db.LaptopModels.Add(mod);
                        db.SaveChanges();
                        //send admin notification
                    }
                    else
                    {
                        var allModels = db.LaptopModels.Where(x => x.brand == company).Select(x => x.model);
                        bool isNewModel = true;
                        foreach (var myModel in allModels)
                        {
                            if (myModel == model)
                            {
                                isNewModel = false;
                            }
                        }
                        if (isNewModel)
                        {
                            LaptopModel mod = new LaptopModel();
                            mod.brand = company;
                            mod.model = model;
                            db.LaptopModels.Add(mod);
                            db.SaveChanges();
                            //send admin notification with user info
                        }
                    }

                    //if (companyName == "HTC" || companyName == "Nokia" || companyName == "Samsung" || companyName == "Iphone")
                    //{
                    //    string[] newModel = model.Split(' ').Skip(1).ToArray();
                    //    model = string.Join("", newModel);
                    //}

                    //var mobiledata = db.LaptopBrands.FirstOrDefault(x => x.Id == company && x.LaptopModels.Any(xu => xu.model.Equals(model)));
                    var laptopdata = db.LaptopModels.FirstOrDefault(x => x.brand == company && x.model == model);
                    mobileAd.laptopId = laptopdata.Id;
                    asp.Ads.Add(ad);
                    db.Ads.Add(ad);
                    mobileAd.adId = ad.Id;
                    ad.LaptopAds.Add(mobileAd);
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        string s = e.ToString();
                    }
                    return RedirectToAction("Index", new { category = "mobiles", subcategory = laptopdata.brand, lowcategory = laptopdata.model, id = ad.Id, title = ad.title });
                }
                TempData["error"] = "You must be logged in to post ad";
                return View("Create", ad);
            }
            TempData["error"] = "Only enter those information about which you are asked";
            return View("Create", ad);
            //ViewBag.postedBy = new SelectList(db.AspNetUsers, "Id", "Email", ad.postedBy);
            //return View(ad);
        }

        // POST: /Electronics/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,category,postedBy,title,description,time")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                if (Request.IsAuthenticated)
                {
                    MobileAd mobileAd= new MobileAd();

                    var isbiding = Request["bidingAllowed"];
                    if (isbiding == "fixedPrice")
                    {
                        var nn = Request["isNegotiable"];
                        if (nn == "on")
                        {
                            ad.isnegotiable = "y";
                        }
                        else
                        {
                            ad.isnegotiable = "n";
                        }
                    }
                    else if (isbiding == "allowBiding")
                    {
                        ad.isnegotiable = "b";
                    }
                    var condition = Request["condition"];
                    if (condition == "new")
                    {
                        mobileAd.condition = "n";
                    }
                    else if (condition == "unboxed")
                    {
                        mobileAd.condition = "b";
                    }
                    else
                    {
                        mobileAd.condition = "u";
                    }
                    mobileAd.sims =int.Parse( Request["sims"]);
                    mobileAd.color = Request["color"];
                    var pp = Request["price"];
                   pp =  pp.Replace(",", string.Empty);
                    if (pp != null && pp != "" ){
                        //if (pp.Substring(pp.Length - 1) == ",")
                        //{
                        //    pp = pp.Remove(pp.Length - 1);
                        //}
                        ad.price =int.Parse( pp);
                    }
                    var company = Request["brand"];
                    var model = Request["model"];
                    ad.time = DateTime.UtcNow;
                    ad.description = System.Web.HttpUtility.HtmlEncode(ad.description);
                    ad.postedBy = User.Identity.GetUserId();
                    AspNetUser asp = db.AspNetUsers.FirstOrDefault(x => x.Id == ad.postedBy);
                    
                    ad.category = "Electronics";
                    //var companyName = model.Split(' ')[0];
                    var allBrands = (db.Mobiles.Select(x => x.Id)).AsEnumerable(); //getBrands
                    bool isNewBrand = true;
                    foreach (var brand in allBrands)
                    {
                        if (brand == company)
                        {
                            isNewBrand = false;
                        }
                    }
                    if (isNewBrand)
                    {
                        Mobile mob = new Mobile();
                        mob.Id = company;
                        db.Mobiles.Add(mob);
                        db.SaveChanges();

                        MobileModel mod = new MobileModel();
                        mod.model = model;
                        mod.Mobile = company;
                        db.MobileModels.Add(mod);
                        db.SaveChanges();
                        //send admin notification
                    }
                    else
                    {
                        var allModels =  db.MobileModels.Where(x => x.Mobile == company).Select(x => x.model);
                        bool isNewModel = true;
                        foreach (var myModel in allModels)
                        {
                            if (myModel == model)
                            {
                                isNewModel = false;
                            }
                        }
                        if (isNewModel)
                        {
                            MobileModel mod = new MobileModel();
                            mod.Mobile = company;
                            mod.model = model;
                            db.MobileModels.Add(mod);
                            db.SaveChanges();
                            //send admin notification with user info
                        }
                    }

                    //if (companyName == "HTC" || companyName == "Nokia" || companyName == "Samsung" || companyName == "Iphone")
                    //{
                    //    string[] newModel = model.Split(' ').Skip(1).ToArray();
                    //    model = string.Join("", newModel);
                    //}

                    //var mobiledata = db.Mobiles.FirstOrDefault(x => x.Id == company && x.MobileModels.Any(xu=>xu.model.Equals(model)));
                    var mobileModel = db.MobileModels.FirstOrDefault(x => x.Mobile == company && x.model == model);
                    //if (mobiledata == null)
                    //{
                    //    Mobile mob = new Mobile();
                    //    mob.Id = companyName;
                    //    db.Mobiles.Add(mob);
                    //    MobileModel mm = new MobileModel();
                    //    mm.Mobile = companyName;
                    //    mm.model = model;
                    //    //send admin notification
                    //}
                    mobileAd.mobileId = mobileModel.Id;
                    asp.Ads.Add(ad);
                    db.Ads.Add(ad);
                    mobileAd.adId = ad.Id;
                    ad.MobileAds.Add(mobileAd);
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        string s = e.ToString();
                    }
                    return RedirectToAction("Index", new {category = "mobiles",subcategory = mobileModel.Mobile,lowcategory = mobileModel.model,id = ad.Id , title = ad.title });
                }
                TempData["error"] = "You must be logged in to post ad";
                return View("Create", ad);
            }
            TempData["error"] = "Only enter those information about which you are asked";
            return View("Create", ad);
            //ViewBag.postedBy = new SelectList(db.AspNetUsers, "Id", "Email", ad.postedBy);
            //return View(ad);
        }
        // GET: /Electronics/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ad ad = db.Ads.Find(id);
            if (ad == null)
            {
                return HttpNotFound();
            }
            ViewBag.postedBy = new SelectList(db.AspNetUsers, "Id", "Email", ad.postedBy);
            return View(ad);
        }

        // POST: /Electronics/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,category,postedBy,title,description,time")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ad).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.postedBy = new SelectList(db.AspNetUsers, "Id", "Email", ad.postedBy);
            return View(ad);
        }

        // GET: /Electronics/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ad ad = db.Ads.Find(id);
            if (ad == null)
            {
                return HttpNotFound();
            }
            return View(ad);
        }

        // POST: /Electronics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ad ad = db.Ads.Find(id);
            db.Ads.Remove(ad);
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
