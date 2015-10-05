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
        public ActionResult Laptops()
        {
            return View();
        }
        public ActionResult Mobiles()
        {
            //db.Ads.Where(x => x.Id.Equals(x.MobileAds.Where(x.));
            return View();
        }
        public ActionResult Home_Appliances()
        {
            return View();
        }
        public ActionResult CreateHomeAppliancesAd()
        {
            Ad ad= new Ad();
            return View(ad);
        }
        public void MyAd(Ad ad,string SaveOrUpdate,string subcategory = null)
        {
            var type = Request["type"];
            var isbiding = Request["bidingAllowed"];
            var condition = Request["condition"];
            var pp = Request["price"];
            string[] prices = pp.Split(',');
            if (type == "sell")
            {
                ad.type = true;
            }
            else
            {
                ad.type = false;
            }

            if (isbiding == "fixedPrice")
            {
                pp = prices[0];
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
                pp = prices[1];
                ad.isnegotiable = "b";
            }

            if (condition == "new")
            {
                ad.condition = "n";
            }
            else if (condition == "unboxed")
            {
                ad.condition = "b";
            }
            else
            {
                ad.condition = "u";
            }
            if (pp != null && pp != "")
            {
                ad.price = int.Parse(pp);
            }
            if (SaveOrUpdate == "Save")
            {
                ad.category = "Electronics";
                ad.subcategory = subcategory;
                ad.time = DateTime.UtcNow;
            }
            else if (SaveOrUpdate == "Update")
            {
                ad.time =DateTime.Parse( Request["time"]);
                ad.category = Request["category"];
                ad.subcategory = Request["subcategory"];
            }
            ad.description = System.Web.HttpUtility.HtmlEncode(ad.description);
            ad.postedBy = User.Identity.GetUserId();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateHomeAppliancesAd([Bind(Include = "Id,category,postedBy,title,description,time")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                if (Request.IsAuthenticated)
                {
                    MyAd(ad,"Save","HomeAppliances");

                    AspNetUser asp = db.AspNetUsers.FirstOrDefault(x => x.Id == ad.postedBy);

                    db.Ads.Add(ad);
                    //tags
                    SaveTags(Request["tags"], ad);
                    //location
                    MyAdLocation(Request["city"], Request["popularPlace"], Request["exectLocation"], ad, "Save");
                    return RedirectToAction("Details", new {  id = ad.Id});
                }
                return View("CreateHomeAppliancesAd", ad);
            }
            return View("CreateHomeAppliancesAd", ad);
            //ViewBag.postedBy = new SelectList(db.AspNetUsers, "Id", "Email", ad.postedBy);
            //return View(ad);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateAd([Bind(Include = "Id,category,postedBy,title,description,time")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                if (Request.IsAuthenticated)
                {
                    if (Request["postedBy"] == User.Identity.GetUserId())
                    {
                        MyAd(ad, "Update");
                        AspNetUser asp = db.AspNetUsers.FirstOrDefault(x => x.Id == ad.postedBy);

                        db.Entry(ad).State = EntityState.Modified;
                        //tags
                        SaveTags(Request["tags"], ad);
                        //location

                        MyAdLocation(Request["city"], Request["popularPlace"], Request["exectLocation"], ad, "Update");
                        return RedirectToAction("Details", new { id = ad.Id });
                    }
                    
                }
                return View("EditAd", ad);
            }
            return View("EditAd", ad);
        }
        public void MyAdLocation(string city, string popularPlace, string exectLocation,Ad ad,string SaveOrUpdate)
        {
            AdsLocation loc = new AdsLocation();
            if (city != null)
            {
                var citydb = db.Cities.FirstOrDefault(x => x.cityName.Equals(city, StringComparison.OrdinalIgnoreCase));
                if (citydb == null)
                {
                    City cit = new City();
                    cit.cityName = city;
                    cit.addedBy = User.Identity.GetUserId();
                    cit.addedBy = User.Identity.GetUserId();
                    cit.addedOn = DateTime.UtcNow;
                    db.Cities.Add(cit);
                    db.SaveChanges();
                    loc.cityId = cit.Id;
                    if (popularPlace != null)
                    {
                        popularPlace pop = new popularPlace();
                        pop.cityId = cit.Id;
                        pop.name = popularPlace;
                        pop.addedBy = User.Identity.GetUserId();
                        pop.addedOn = DateTime.UtcNow;
                        db.popularPlaces.Add(pop);
                        db.SaveChanges();
                        loc.popularPlaceId = pop.Id;
                    }
                }
                else
                {
                    loc.cityId = citydb.Id;
                    if (popularPlace != null)
                    {
                        var ppp = db.popularPlaces.FirstOrDefault(x => x.City.cityName.Equals(city, StringComparison.OrdinalIgnoreCase) && x.name.Equals(popularPlace, StringComparison.OrdinalIgnoreCase));
                        if (ppp == null)
                        {
                            popularPlace pop = new popularPlace();
                            pop.cityId = citydb.Id;
                            pop.name = popularPlace;
                            pop.addedBy = User.Identity.GetUserId();
                            pop.addedOn = DateTime.UtcNow;
                            db.popularPlaces.Add(pop);
                            db.SaveChanges();
                            loc.popularPlaceId = pop.Id;
                        }
                        else
                        {
                            loc.popularPlaceId = ppp.Id;
                        }
                    }
                }
                loc.exectLocation = exectLocation;
                loc.Id = ad.Id;
                if (SaveOrUpdate == "Save")
                {
                    db.AdsLocations.Add(loc);
                }
                else if (SaveOrUpdate == "Update")
                {
                    db.Entry(loc).State = EntityState.Modified;
                }
                db.SaveChanges();
            }
        }
        public ActionResult EditAd(int id)
        {
            Ad ad = db.Ads.Find(id);
            if (ad == null)
            {
                return HttpNotFound();
            }
            return View(ad);
        }
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
                    var mobileAds = ad.MobileAd;
                    ViewBag.ad = mobileAds;
                    ViewBag.category = "mobiles";
                    ViewBag.adId = id;
                }
                return View("Details",ad);
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        //[HttpPost]
        public ActionResult Edit(int id)
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
        public ActionResult EditLaptopAd(int id)
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
        public ActionResult Update([Bind(Include = "Id,category,postedBy,title,description,time")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                if (Request.IsAuthenticated)
                {
                   
                    if (Request["postedBy"] == User.Identity.GetUserId())
                    {
                        MyAd(ad, "Update");

                        MobileAd mobileAd = new MobileAd();

                        mobileAd.sims = Request["sims"];
                        mobileAd.color = Request["color"];

                        var company = Request["brand"];
                        var model = Request["model"];
                        AspNetUser asp = db.AspNetUsers.FirstOrDefault(x => x.Id == ad.postedBy);

                        var allBrands = (db.Mobiles.Select(x => x.brand)).AsEnumerable(); //getBrands
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
                            mob.brand = company;
                            mob.addedBy = User.Identity.GetUserId();
                            mob.time = DateTime.UtcNow;
                            db.Mobiles.Add(mob);
                            db.SaveChanges();

                            MobileModel mod = new MobileModel();
                            mod.model = model;
                            mod.brandId = mob.Id;
                            mod.time = DateTime.UtcNow;
                            mod.addedBy = User.Identity.GetUserId();
                            db.MobileModels.Add(mod);
                            db.SaveChanges();
                        }
                        else
                        {
                            var allModels = db.MobileModels.Where(x => x.Mobile.brand == company).Select(x => x.model);
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
                                var brandId = db.Mobiles.FirstOrDefault(x => x.brand.Equals(company));
                                MobileModel mod = new MobileModel();
                                mod.brandId = brandId.Id;
                                mod.model = model;
                                mod.time = DateTime.UtcNow;
                                mod.addedBy = User.Identity.GetUserId();
                                db.MobileModels.Add(mod);
                                db.SaveChanges();
                            }
                        }

                        //tags
                        SaveTags(Request["tags"], ad);

                        var mobileModel = db.MobileModels.FirstOrDefault(x => x.Mobile.brand == company && x.model == model);
                        mobileAd.mobileId = mobileModel.Id;
                        //asp.Ads.Add(ad);

                        db.Entry(ad).State = EntityState.Modified;
                        db.SaveChanges();
                        //db.Ads.Add(ad);
                        mobileAd.Id = ad.Id;
                        db.Entry(mobileAd).State = EntityState.Modified;
                        //ad.MobileAds.Add(mobileAd);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            string sss = e.ToString();
                        }
                        //location
                        MyAdLocation(Request["city"], Request["popularPlace"], Request["exectLocation"], ad, "Update");
                        return RedirectToAction("Index", new { category = "mobiles", subcategory = mobileModel.Mobile.brand, lowcategory = mobileModel.model, id = ad.Id, title = ad.title });
                    }
                }
                return View("Create", ad);
            }
            return View("Create", ad);
        }
        public void SaveTags(string s,Ad ad)
        {
            //string s = Request["tags"];
            string[] values = s.Split(',');
            Tag[] tags = new Tag[values.Length];
            AdTag[] qt = new AdTag[values.Length];
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
                    qt[i] = new AdTag();
                    qt[i].adId = ad.Id;
                    qt[i].tagId = tags[i].Id;
                    db.AdTags.Add(qt[i]);
                }
            }
        }
        public ActionResult UpdateLaptopAd([Bind(Include = "Id,category,postedBy,title,description,time")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                if (Request.IsAuthenticated)
                {
                    if (Request["postedBy"] == User.Identity.GetUserId())
                    {
                        MyAd(ad, "Update");
                        LaptopAd mobileAd = new LaptopAd();

                        mobileAd.color = Request["color"];
                        var company = Request["brand"];
                        var model = Request["model"];
                        AspNetUser asp = db.AspNetUsers.FirstOrDefault(x => x.Id == ad.postedBy);

                        var allBrands = (db.LaptopBrands.Select(x => x.brand)).AsEnumerable(); //getBrands
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
                            LaptopBrand mob = new LaptopBrand();
                            mob.brand = company;
                            mob.addedBy = User.Identity.GetUserId();
                            mob.time = DateTime.UtcNow;
                            db.LaptopBrands.Add(mob);
                            db.SaveChanges();

                            LaptopModel mod = new LaptopModel();
                            mod.model = model;
                            mod.brandId = mob.Id;
                            mod.time = DateTime.UtcNow;
                            mod.addedBy = User.Identity.GetUserId();
                            db.LaptopModels.Add(mod);
                            db.SaveChanges();
                        }
                        else
                        {
                            var allModels = db.LaptopModels.Where(x => x.LaptopBrand.brand == company).Select(x => x.model);
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
                                var brandId = db.LaptopBrands.First(x => x.brand.Equals(company));
                                LaptopModel mod = new LaptopModel();
                                mod.brandId = brandId.Id;
                                mod.model = model;
                                mod.time = DateTime.UtcNow;
                                mod.addedBy = User.Identity.GetUserId();
                                db.LaptopModels.Add(mod);
                                db.SaveChanges();
                            }
                        }

                        //tags
                        SaveTags(Request["tags"], ad);

                        var mobileModel = db.LaptopModels.FirstOrDefault(x => x.LaptopBrand.brand == company && x.model == model);
                        mobileAd.laptopId = mobileModel.Id;
                        //asp.Ads.Add(ad);

                        db.Entry(ad).State = EntityState.Modified;
                        db.SaveChanges();
                        //db.Ads.Add(ad);
                        mobileAd.Id = ad.Id;
                        db.Entry(mobileAd).State = EntityState.Modified;
                        //ad.MobileAds.Add(mobileAd);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            string sss = e.ToString();
                        }
                        //location
                        MyAdLocation(Request["city"], Request["popularPlace"], Request["exectLocation"], ad, "Update");
                        return RedirectToAction("Index", new { category = "mobiles", subcategory = mobileModel.LaptopBrand.brand, lowcategory = mobileModel.model, id = ad.Id, title = ad.title });
                    }
                }
                return View("Create", ad);
            }
            return View("Create", ad);
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

        // GET: /Electronics/Create
        public ActionResult Create()
        {
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
                    MyAd(ad, "Save");
                    LaptopAd mobileAd = new LaptopAd();
                    mobileAd.color = Request["color"];
                    
                    var company = Request["brand"];
                    var model = Request["model"];
                    
                    AspNetUser asp = db.AspNetUsers.FirstOrDefault(x => x.Id == ad.postedBy);

                    //var companyName = model.Split(' ')[0];
                    var allBrands = (db.LaptopBrands.Select(x => x.brand)).AsEnumerable(); //getBrands
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
                        mob.brand = company;
                        mob.addedBy = User.Identity.GetUserId();
                        mob.time = DateTime.UtcNow;
                        db.LaptopBrands.Add(mob);
                        db.SaveChanges();

                        LaptopModel mod = new  LaptopModel();
                        mod.model = model;
                        mod.brandId = mob.Id;
                        mod.addedBy = User.Identity.GetUserId();
                        mod.time = DateTime.UtcNow;
                        db.LaptopModels.Add(mod);
                        db.SaveChanges();
                        //send admin notification
                    }
                    else
                    {
                        var allModels = db.LaptopModels.Where(x => x.LaptopBrand.brand == company).Select(x => x.model);
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
                            var brandId = db.LaptopBrands.First(x => x.brand.Equals(company));
                            LaptopModel mod = new LaptopModel();
                            mod.brandId = brandId.Id;
                            mod.model = model;
                            mod.addedBy = User.Identity.GetUserId();
                            mod.time = DateTime.UtcNow;
                            db.LaptopModels.Add(mod);
                            db.SaveChanges();
                            //send admin notification with user info
                        }
                    }var laptopdata = db.LaptopModels.FirstOrDefault(x => x.LaptopBrand.brand == company && x.model == model);
                    mobileAd.laptopId = laptopdata.Id;
                   // asp.Ads.Add(ad); I changed this
                    db.Ads.Add(ad);
                    //tags
                    SaveTags(Request["tags"], ad);
                    mobileAd.Id = ad.Id;
                   // ad.LaptopAd.Add(mobileAd);
                    db.LaptopAds.Add(mobileAd);
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        string ss = e.ToString();
                    }
                    //location
                    MyAdLocation(Request["city"], Request["popularPlace"], Request["exectLocation"], ad,"Save");
                    return RedirectToAction("Index", new { category = "mobiles", subcategory = laptopdata.LaptopBrand.brand, lowcategory = laptopdata.model, id = ad.Id, title = ad.title });
                }
                TempData["error"] = "You must be logged in to post ad";
                return View("Create", ad);
            }
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
                    MyAd(ad, "Save");
                    MobileAd mobileAd= new MobileAd();
                    mobileAd.sims =Request["sims"];
                    mobileAd.color = Request["color"];
                    
                    var company = Request["brand"];
                    var model = Request["model"];
                    
                    AspNetUser asp = db.AspNetUsers.FirstOrDefault(x => x.Id == ad.postedBy);
                    
                    var allBrands = (db.Mobiles.Select(x => x.brand)).AsEnumerable(); //getBrands
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
                        mob.brand = company;
                        mob.addedBy = User.Identity.GetUserId();
                        mob.time = DateTime.UtcNow;
                        db.Mobiles.Add(mob);
                        db.SaveChanges();

                        MobileModel mod = new MobileModel();
                        mod.model = model;
                        mod.brandId = mob.Id;
                        mod.time = DateTime.UtcNow;
                        mod.addedBy = User.Identity.GetUserId();
                        db.MobileModels.Add(mod);
                        db.SaveChanges();
                    }
                    else
                    {
                        var allModels =  db.MobileModels.Where(x => x.Mobile.brand == company).Select(x => x.model);
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
                            var brandId = db.Mobiles.FirstOrDefault(x => x.brand.Equals(company));
                            MobileModel mod = new MobileModel();
                            mod.brandId = brandId.Id;
                            mod.model = model;
                            mod.addedBy = User.Identity.GetUserId();
                            mod.time = DateTime.UtcNow;
                            db.MobileModels.Add(mod);
                            db.SaveChanges();
                        }
                    }
                    var mobileModel = db.MobileModels.FirstOrDefault(x => x.Mobile.brand == company && x.model == model);
                    mobileAd.mobileId = mobileModel.Id;
                    asp.Ads.Add(ad);
                    db.Ads.Add(ad);
                    //tags
                    SaveTags(Request["tags"], ad);

                    mobileAd.Id = ad.Id;
                    db.MobileAds.Add(mobileAd);
                    //ad.MobileAd.a(mobileAd);
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        string sbs = e.ToString();
                    }
                    //location
                    MyAdLocation(Request["city"], Request["popularPlace"], Request["exectLocation"], ad, "Save");
                    return RedirectToAction("Index", new {category = "mobiles",subcategory = mobileModel.Mobile.brand,lowcategory = mobileModel.model,id = ad.Id , title = ad.title });
                }
                TempData["error"] = "You must be logged in to post ad";
                return View("Create", ad);
            }
            TempData["error"] = "Only enter those information about which you are asked";
            return View("Create", ad);
            //ViewBag.postedBy = new SelectList(db.AspNetUsers, "Id", "Email", ad.postedBy);
            //return View(ad);
        }
        

        // POST: /Electronics/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        

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
