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
    public class VehiclesController : Controller
    {
        private Entities db = new Entities();

        // GET: /Vehicles/
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
                else if (category == "cars")
                {
                    ViewBag.adId = id;
                }
                return View("../Electronics/Details", new { id = id});
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }


        // GET: /Vehicles/Details/5
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
            ViewBag.adId = id;
            return View();
        }

        // GET: /Vehicles/Create
        public ActionResult CreateCarAd()
        {
            Ad ad = new Ad();
            return View(ad);
        }

        // POST: /Vehicles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateCarAd([Bind(Include="Id,category,postedBy,title,description,time,price,isnegotiable")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                if (Request.IsAuthenticated)
                {
                    CarAd mobileAd = new CarAd();

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
                    mobileAd.year = short.Parse( Request["year"]);
                    mobileAd.kmDriven = int.Parse(Request["kmDriven"]);
                    mobileAd.fuelType = Request["fuelType"];
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

                    ad.category = "Vehicles";
                    //var companyName = model.Split(' ')[0];
                    var allBrands = (db.CarBrands.Select(x => x.Id)).AsEnumerable(); //getBrands
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
                        CarBrand mob = new CarBrand();
                        mob.Id = company;
                        db.CarBrands.Add(mob);
                        db.SaveChanges();

                        CarModel mod = new  CarModel();
                        mod.model = model;
                        mod.brand = company;
                        db.CarModels.Add(mod);
                        db.SaveChanges();
                        //send admin notification
                    }
                    else
                    {
                        var allModels = db.CarModels.Where(x => x.brand == company).Select(x => x.model);
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
                            CarModel mod = new  CarModel();
                            mod.brand = company;
                            mod.model = model;
                            db.CarModels.Add(mod);
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
                    var laptopdata = db.CarModels.FirstOrDefault(x => x.brand == company && x.model == model);
                    mobileAd.carId = laptopdata.Id;
                    asp.Ads.Add(ad);
                    db.Ads.Add(ad);
                    mobileAd.adId = ad.Id;
                    ad.CarAds.Add(mobileAd);
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        string s = e.ToString();
                    }
                    return RedirectToAction("Index", new { category = "cars", subcategory = laptopdata.brand, lowcategory = laptopdata.model, id = ad.Id, title = ad.title });
                }
                TempData["error"] = "You must be logged in to post ad";
                return View("Create", ad);
            }
            TempData["error"] = "Only enter those information about which you are asked";
            return View("Create", ad);
        }

        // GET: /Vehicles/Edit/5
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
            return View(ad);
        }

        // POST: /Vehicles/Edit/5
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
            return View(ad);
        }

        // GET: /Vehicles/Delete/5
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

        // POST: /Vehicles/Delete/5
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
