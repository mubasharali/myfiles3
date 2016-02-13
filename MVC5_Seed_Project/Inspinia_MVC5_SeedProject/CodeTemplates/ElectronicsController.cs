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
using System.Text;
using Newtonsoft.Json;
namespace Inspinia_MVC5_SeedProject.CodeTemplates
{
    public class FileName
    {
        public string fileName;
        public string fileId;
    }
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
        public ActionResult ComputersLaptops()
        {
            return View("Laptops");
        }
       
        public void PostAdByCompanyPage(int adId,bool update = false)
        {
            var postAdUsing = System.Web.HttpContext.Current.Request["postAdUsing"];
            if (postAdUsing != null)
            {
                if (update)
                {
                    var comads = db.CompanyAds.Find(adId);
                    if(comads != null)
                    {
                        db.CompanyAds.Remove(comads);
                        db.SaveChanges();
                    }
                }
                if (postAdUsing != "on")
                {
                    var companyId = db.Companies.FirstOrDefault(x => x.title.Equals(postAdUsing)).Id;
                    CompanyAd comAd = new CompanyAd();
                    comAd.companyId = companyId;
                    comAd.adId = adId;
                    db.CompanyAds.Add(comAd);
                    db.SaveChanges();
                }
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateLaptopAd([Bind(Include = "Id,category,postedBy,title,description,time")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                if (Request.IsAuthenticated)
                {
                    FileName[] fileNames = JsonConvert.DeserializeObject<FileName[]>(Request["files"]);
                    MyAd(ref ad, "Save","Laptops");
                    LaptopAd mobileAd = new LaptopAd();
                    mobileAd.color = Request["color"];

                    mobileAd.laptopId = SaveLaptopBrandModel(ad);
                    db.Ads.Add(ad);
                    //tags
                    SaveTags(Request["tags"],ref ad);
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
                    PostAdByCompanyPage(ad.Id);
                    ReplaceAdImages( ref ad, fileNames);
                    //location
                    MyAdLocation(Request["city"], Request["popularPlace"], Request["exectLocation"],ref ad, "Save");
                    return RedirectToAction("Details",new {id = ad.Id, title = ad.title });
                    //return RedirectToAction("Index", new { category = "mobiles", subcategory = laptopdata.LaptopBrand.brand, lowcategory = laptopdata.model, id = ad.Id, title = ad.title });
                }
                return View("Create", ad);
            }
            return View("Create", ad);
        }

        public ActionResult CreateLaptopAccessoriesAd()
        {
            if (Request.IsAuthenticated)
            {
                Ad ad = new Ad();
                return View(ad);
            }
            return RedirectToAction("Index", "Home");
        }
        public ActionResult EditLaptopAccessoriesAd(int id)
        {
            if (Request.IsAuthenticated)
            {
                Ad ad = db.Ads.Find(id);
                if (ad.postedBy == User.Identity.GetUserId())
                {
                    return View(ad);
                }
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateLaptopAccessoriesAd([Bind(Include = "Id,category,postedBy,title,description,time")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                if (Request.IsAuthenticated)
                {
                    FileName[] fileNames = JsonConvert.DeserializeObject<FileName[]>(Request["files"]);
                    MyAd(ref ad, "Save", "LaptopAccessories");
                    db.Ads.Add(ad);
                    db.SaveChanges();
                    PostAdByCompanyPage(ad.Id);
                    LaptopAd mobileAd = new  LaptopAd();
                    mobileAd.color = Request["color"];

                    mobileAd.laptopId = SaveLaptopBrandModel( ad);

                    //tags
                    SaveTags(Request["tags"], ref ad);

                    mobileAd.Id = ad.Id;
                    db.LaptopAds.Add(mobileAd);
                    ReplaceAdImages(ref ad, fileNames);
                    //location
                    MyAdLocation(Request["city"], Request["popularPlace"], Request["exectLocation"], ref ad, "Save");
                    db.SaveChanges();
                    return RedirectToAction("../Electronics/Details/" + ad.Id + "/" + ad.title);
                }
                return View("Create", ad);
            }
            return View("Create", ad);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateLaptopAccessoriesAd([Bind(Include = "Id,category,postedBy,title,description,time")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                if (Request.IsAuthenticated)
                {
                    if (Request["postedBy"] == User.Identity.GetUserId())
                    {
                        FileName[] fileNames = JsonConvert.DeserializeObject<FileName[]>(Request["files"]);
                        MyAd(ref ad, "Update");

                        db.Entry(ad).State = EntityState.Modified;
                        db.SaveChanges();
                        LaptopAd mobileAd = new LaptopAd();
                        mobileAd.color = Request["color"];

                        mobileAd.laptopId = SaveLaptopBrandModel(ad);

                       mobileAd.laptopId = SaveLaptopBrandModel( ad);
                        mobileAd.Id = ad.Id;
                        db.Entry(mobileAd).State = EntityState.Modified;

                        //tags
                        SaveTags(Request["tags"], ref ad, "update");
                        //location

                        MyAdLocation(Request["city"], Request["popularPlace"], Request["exectLocation"], ref ad, "Update");
                        ReplaceAdImages(ref ad, fileNames);
                        db.SaveChanges();
                        return RedirectToAction("../Details/" + ad.Id + "/" + ad.title);
                    }

                }
                return View("EditLaptopAccessoriesAd", ad);
            }
            return View("EditLaptopAccessoriesAd", ad);
        }

        public void ReplaceAdImages(ref Ad ad, FileName[] filenames)
        {
            string newFileName = "";
            int count = 1;
            var id = ad.Id;
            var imaa = db.AdImages.Where(x => x.adId.Equals(id)).Count();
            //var imaa = db.Ads.Find(id).AdImages.Count;
            //foreach (var img in imaa)
            //{
            //    System.IO.File.Delete(Server.MapPath(@"~\Images\Ads\" + ad.Id + "_" + count++ + img.imageExtension));
            //    db.AdImages.Remove(img);
            //}
            count = imaa + 1;
            for (int i = 1; i < filenames.Length; i++)
            {
                string filename = System.Web.HttpContext.Current.Server.MapPath("~/Images/Ads/" +  filenames[i].fileName );
                string extension = System.IO.Path.GetExtension(filenames[i].fileName);
                newFileName = ad.Id.ToString() + "_" + count + extension;
                if (System.IO.File.Exists(filename))
                {
                    if ( ! (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath("~/Images/Ads/" + newFileName)) ))
                    {
                        System.IO.File.Move(filename, System.Web.HttpContext.Current.Server.MapPath("~/Images/Ads/" + newFileName));
                        System.IO.File.Delete(filename);
                        AdImage image = new AdImage();
                        image.imageExtension = extension;
                        image.adId = ad.Id;
                        db.AdImages.Add(image);
                        try {
                            db.SaveChanges();
                        }catch(Exception e)
                        {
                            string s = e.ToString();
                        }
                        count++;
                    }
                }
            }
            
            //try {
            //    db.SaveChanges();
            //}
            //catch(Exception e)
            //{
            //    string s = e.ToString();
            //}
        }
        
        [HttpPost]
        public ActionResult FileUploadHandler()
        {
            string[] fileNames = null;
            bool canpass = true;
            string filename = "";
            
            for (int i = 0; i < Request.Files.Count; i++)
            {
                if (canpass)
                {
                    fileNames = new string[Request.Files.Count];
                    canpass = false;
                }
                try
                {
                    HttpPostedFileBase file = Request.Files[i];
                    string extension = System.IO.Path.GetExtension(file.FileName);
                    filename = "temp" + DateTime.UtcNow.Ticks + extension;
                    file.SaveAs(Server.MapPath("~/Images/Ads/" + filename));
                    fileNames[i] = filename;
                }
                catch (Exception ex)
                {
                    return Json(new { Message = "Error in saving file" });
                }

            }
            return Json(fileNames);
        }


        public int SaveLaptopBrandModel(Ad ad)
        {
            ad.status = "a";
            var company = Request["brand"];
            var model = Request["model"];
            if (company != null && company != "")
            {
                company = company.Trim();
                model = model.Trim();
            }
            if (true) //company != null
            {

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
                    mob.addedBy = System.Web.HttpContext.Current.User.Identity.GetUserId();
                    mob.time = DateTime.UtcNow;
                    if (company == null || company == "")
                    {
                        mob.status = "a";
                    }
                    else
                    {
                        mob.status = "p";
                    }
                    db.LaptopBrands.Add(mob);
                    db.SaveChanges();

                    LaptopModel mod = new LaptopModel();
                    mod.model = model;
                    mod.brandId = mob.Id;
                    mod.time = DateTime.UtcNow;
                    if (model == null || model == "")
                    {
                        mod.status = "a";
                    }
                    else
                    {
                        mod.status = "p";
                    }
                    mod.addedBy = System.Web.HttpContext.Current.User.Identity.GetUserId();
                    db.LaptopModels.Add(mod);
                    db.SaveChanges();
                    ad.status = "p";
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
                        ad.status = "p";
                        var brandId = db.LaptopBrands.FirstOrDefault(x => x.brand.Equals(company));
                        LaptopModel mod = new LaptopModel();
                        mod.brandId = brandId.Id;
                        mod.model = model;
                        if (model == null || model == "")
                        {
                            mod.status = "a";
                        }
                        else
                        {
                            mod.status = "p";
                        }
                        mod.addedBy = System.Web.HttpContext.Current.User.Identity.GetUserId();
                        mod.time = DateTime.UtcNow;
                        db.LaptopModels.Add(mod);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            string s = e.ToString();
                        }
                    }
                }
                var laptopModel = db.LaptopModels.FirstOrDefault(x => x.LaptopBrand.brand == company && x.model == model);
                return laptopModel.Id;
            }
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
        public void MyAd(ref Ad ad,string SaveOrUpdate,string cateogry = null,string subcategory = null)
        {
            var type = System.Web.HttpContext.Current.Request["type"];
            var isbiding = System.Web.HttpContext.Current.Request["bidingAllowed"];
            var condition = System.Web.HttpContext.Current.Request["condition"];
            var pp = System.Web.HttpContext.Current.Request["price"];
            string[] prices = pp.Split(',');   //exception: object reference not set to instance of object
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
                var nn = System.Web.HttpContext.Current.Request["isNegotiable"];
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
                ad.category = cateogry;
                ad.subcategory = subcategory;
                ad.time = DateTime.UtcNow;
            }
            else if (SaveOrUpdate == "Update")
            {
                ad.time =DateTime.Parse(System.Web.HttpContext.Current.Request["time"]);
                ad.category = System.Web.HttpContext.Current.Request["category"];
                ad.subcategory = System.Web.HttpContext.Current.Request["subcategory"];
            }
            ad.description = System.Web.HttpUtility.HtmlEncode(ad.description);
            ad.postedBy = System.Web.HttpContext.Current.User.Identity.GetUserId();

        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateHomeAppliancesAd([Bind(Include = "Id,category,postedBy,title,description,time")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                if (Request.IsAuthenticated)
                {
                    MyAd(ref ad,"Save","Electronics","HomeAppliances");

                    AspNetUser asp = db.AspNetUsers.FirstOrDefault(x => x.Id == ad.postedBy);

                    db.Ads.Add(ad);
                    //tags
                    SaveTags(Request["tags"],ref ad);
                    //location
                    MyAdLocation(Request["city"], Request["popularPlace"], Request["exectLocation"],ref ad, "Save");
                    return RedirectToAction("Details", new {  id = ad.Id});
                }
                return View("CreateHomeAppliancesAd",ad);
            }
            return View("CreateHomeAppliancesAd", ad);
            //ViewBag.postedBy = new SelectList(db.AspNetUsers, "Id", "Email", ad.postedBy);
            //return View(ad);
        }
        
        public void MyAdLocation(string city, string popularPlace, string exectLocation,ref Ad ad,string SaveOrUpdate)
        {
            AdsLocation loc = new AdsLocation();
            if (city != null)
            {
                var citydb = db.Cities.FirstOrDefault(x => x.cityName.Equals(city, StringComparison.OrdinalIgnoreCase));
                if (citydb == null)
                {
                    City cit = new City();
                    cit.cityName = city;
                    cit.addedBy = System.Web.HttpContext.Current.User.Identity.GetUserId();
                    cit.addedBy = System.Web.HttpContext.Current.User.Identity.GetUserId();
                    cit.addedOn = DateTime.UtcNow;
                    db.Cities.Add(cit);
                    db.SaveChanges();
                    loc.cityId = cit.Id;
                    if (popularPlace != null)
                    {
                        popularPlace pop = new popularPlace();
                        pop.cityId = cit.Id;
                        pop.name = popularPlace;
                        pop.addedBy = System.Web.HttpContext.Current.User.Identity.GetUserId();
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
                            pop.addedBy = System.Web.HttpContext.Current.User.Identity.GetUserId();
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
                    ad.AdsLocation = loc;
                   // db.AdsLocations.Add(loc);
                }
                else if (SaveOrUpdate == "Update")
                {
                    db.Entry(loc).State = EntityState.Modified;
                }
                db.SaveChanges();
            }
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
        public static string RemapInternationalCharToAscii(char c)
        {
            string s = c.ToString().ToLowerInvariant();
            if ("àåáâäãåą".Contains(s))
            {
                return "a";
            }
            else if ("èéêëę".Contains(s))
            {
                return "e";
            }
            else if ("ìíîïı".Contains(s))
            {
                return "i";
            }
            else if ("òóôõöøőð".Contains(s))
            {
                return "o";
            }
            else if ("ùúûüŭů".Contains(s))
            {
                return "u";
            }
            else if ("çćčĉ".Contains(s))
            {
                return "c";
            }
            else if ("żźž".Contains(s))
            {
                return "z";
            }
            else if ("śşšŝ".Contains(s))
            {
                return "s";
            }
            else if ("ñń".Contains(s))
            {
                return "n";
            }
            else if ("ýÿ".Contains(s))
            {
                return "y";
            }
            else if ("ğĝ".Contains(s))
            {
                return "g";
            }
            else if (c == 'ř')
            {
                return "r";
            }
            else if (c == 'ł')
            {
                return "l";
            }
            else if (c == 'đ')
            {
                return "d";
            }
            else if (c == 'ß')
            {
                return "ss";
            }
            else if (c == 'Þ')
            {
                return "th";
            }
            else if (c == 'ĥ')
            {
                return "h";
            }
            else if (c == 'ĵ')
            {
                return "j";
            }
            else
            {
                return "";
            }
        }
        public static string URLFriendly(string title)
        {
            if (title == null) return "";

            const int maxlen = 80;
            int len = title.Length;
            bool prevdash = false;
            var sb = new StringBuilder(len);
            char c;

            for (int i = 0; i < len; i++)
            {
                c = title[i];
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
                {
                    sb.Append(c);
                    prevdash = false;
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    // tricky way to convert to lowercase
                    sb.Append((char)(c | 32));
                    prevdash = false;
                }
                else if (c == ' ' || c == ',' || c == '.' || c == '/' ||
                    c == '\\' || c == '-' || c == '_' || c == '=')
                {
                    if (!prevdash && sb.Length > 0)
                    {
                        sb.Append('-');
                        prevdash = true;
                    }
                }
                else if ((int)c >= 128)
                {
                    int prevlen = sb.Length;
                    sb.Append(RemapInternationalCharToAscii(c));
                    if (prevlen != sb.Length) prevdash = false;
                }
                if (i == maxlen) break;
            }

            if (prevdash)
                return sb.ToString().Substring(0, sb.Length - 1);
            else
                return sb.ToString();
        }
        public void SaveTags(string s,ref Ad ad,string addOrUpdate = "add")
        {
            if(addOrUpdate == "update")
            {
                var adid = ad.Id;
                var adtags = db.AdTags.Where(x => x.adId.Equals(adid)).ToList();
                foreach (var cc in adtags)
                {
                    db.AdTags.Remove(cc);
                }
                 db.SaveChanges();
            }
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
                    qt[i] = new AdTag();
                    qt[i].adId = ad.Id;
                    qt[i].tagId = tags[i].Id;
                    db.AdTags.Add(qt[i]);
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
        }
        public ActionResult UpdateLaptopAd([Bind(Include = "Id,category,postedBy,title,description,time")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                if (Request.IsAuthenticated)
                {
                    if (Request["postedBy"] == User.Identity.GetUserId())
                    {
                        FileName[] fileNames = JsonConvert.DeserializeObject<FileName[]>(Request["files"]);
                        MyAd(ref ad, "Update");
                        LaptopAd mobileAd = new LaptopAd();

                        mobileAd.color = Request["color"];
                        var company = Request["brand"];
                        var model = Request["model"];

                        SaveLaptopBrandModel(ad);
                        //tags
                        SaveTags(Request["tags"],ref ad,"update");

                        var mobileModel = db.LaptopModels.FirstOrDefault(x => x.LaptopBrand.brand == company && x.model == model);
                        mobileAd.laptopId = mobileModel.Id;
                       
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
                        MyAdLocation(Request["city"], Request["popularPlace"], Request["exectLocation"],ref ad, "Update");
                        ReplaceAdImages(ref ad, fileNames);
                        return RedirectToAction("Details", new { id = ad.Id, title = ad.title });
                    }
                }
                return View("Create", ad);
            }
            return View("Create", ad);
        }
        [Route("Details/{id?}/{title?}")]
        public ActionResult Details(int? id,string title = null)
        {
            ViewBag.adId = id;
            ViewBag.title = db.Ads.Find(id).title;
            return View();
        }
        
        // GET: /Electronics/Create
        
        public ActionResult CreateLaptopAd()
        {
            Ad ad = new Ad();
            return View(ad);
        }
        
        
        // POST: /Electronics/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        
        

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
