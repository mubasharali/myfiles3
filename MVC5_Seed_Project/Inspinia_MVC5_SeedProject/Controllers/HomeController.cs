﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNet.Identity;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Data.Entity.Validation;
using Inspinia_MVC5_SeedProject.Models;
using Inspinia_MVC5_SeedProject.CodeTemplates;
namespace Inspinia_MVC5_SeedProject.Controllers
{
   // [InitializeSimpleMembership]
    public class HomeController : Controller
    {
        public Entities db = new Entities();
        ElectronicsController electronicController = new ElectronicsController();
        public string subdomainName
        {
            get
            {
                string s = Request.Url.Host;
                var index = s.IndexOf(".");

                if (index < 0)
                {
                    return null;
                }
                var sub = s.Split('.')[0];
                if (sub == "www" || sub == "localhsot")
                {
                    return null;
                }
                return sub;
            }
        }

        #region -- Robots() Method --
        public ActionResult Robots()
        {
            Response.ContentType = "text/plain";
            return View();
        }
        #endregion

        [Route("Feedback-contact")]
        public ActionResult Feedback()
        {
            return View();
        }
       
        public ActionResult Create(string category, string subcategory = null)
        {
            if (Request.IsAuthenticated)
            {
                if (checkCategory(category, subcategory))
                {
                    ViewBag.category = category;
                    ViewBag.subcategory = subcategory;
                    Ad ad = new Ad();
                    return View(ad);
                }
                return HttpNotFound();
            }
            return RedirectToAction("Register", "Account");
        }
        public bool checkCategory(string category, string subcategory)
        {
            if (category == "Vehicles")
            {
                if (subcategory == "commerical-vehicles" || subcategory == "vehicles-for-rent" || subcategory == "other-vehicles" || subcategory == "spare-parts")
                {
                    return true;
                }
            }
            else if (category == "RealEstate")
            {
                string[] subcategories = {"apartment", "house", "plot & land", "Shop", "Office", "PG & Flatmates", "other commerical places"}; //reference over Realestate/create page + RealEstateSearch.js
                foreach (var subcat in subcategories)
                {
                    if (subcategory == subcat)
                    {
                        return true;
                    }
                }
            }
            else if (category == "Electronics")
            {
                string[] subcategories = { "TV-DVD-Multimedia", "Games", "Home-Appliances", "Other-Electronics" };
                foreach (var subcat in subcategories)
                {
                    if (subcategory == subcat)
                    {
                        return true;
                    }
                }
            }
            else if (category == "Animals")
            {
                return true;
            }
            return false;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,category,subcategory,postedBy,title,description,time,price,isnegotiable")] Ad ad)
        {

            if (ModelState.IsValid)
            {
                if (Request.IsAuthenticated)
                {
                    if (!checkCategory(ad.category, ad.subcategory))
                    {
                        return RedirectToAction("CreateAd", "Home");
                    }
                    //string tempId = Request["tempId"];
                    FileName[] fileNames = JsonConvert.DeserializeObject<FileName[]>(Request["files"]);
                    electronicController.MyAd(ref ad, "Save", ad.category, ad.subcategory);
                    db.Ads.Add(ad);
                    db.SaveChanges();


                    electronicController.PostAdByCompanyPage(ad.Id);


                    //images
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        string sbs = e.ToString();
                    }
                    //tags
                    electronicController.SaveTags(Request["tags"], ref ad);

                    electronicController.ReplaceAdImages(ref ad, fileNames);
                    //location
                    electronicController.MyAdLocation(Request["city"], Request["popularPlace"], Request["exectLocation"], ref ad, "Save");
                    db.SaveChanges();
                    return RedirectToAction("Details", "Electronics", new { id = ad.Id, title = ElectronicsController.URLFriendly(ad.title) });
                }
                return RedirectToAction("Register", "Account");
            }
            TempData["error"] = "Only enter those information about which you are asked";
            return View("Create", ad);
        }
        public async Task<ActionResult> Edit(int? id)
        {
            if (Request.IsAuthenticated)
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
            return RedirectToAction("Register", "Account");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,category,subcategory,postedBy,title,description,time,price,isnegotiable")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                if (Request.IsAuthenticated)
                {
                    var ab = Request["postedBy"];
                    var iddd = User.Identity.GetUserId();
                    if (Request["postedBy"] == User.Identity.GetUserId())
                    {
                        FileName[] fileNames = JsonConvert.DeserializeObject<FileName[]>(Request["files"]);
                        electronicController.MyAd(ref ad, "Update", ad.category , ad.subcategory);


                        electronicController.PostAdByCompanyPage(ad.Id, true);

                        db.SaveChanges();
                        //tags
                        electronicController.SaveTags(Request["tags"], ref ad, "update");


                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            string sss = e.ToString();
                        }
                        //location

                        db.Entry(ad).State = EntityState.Modified;
                        db.SaveChanges();

                        electronicController.ReplaceAdImages(ref ad, fileNames);
                        electronicController.MyAdLocation(Request["city"], Request["popularPlace"], Request["exectLocation"], ref ad, "Update");

                        return RedirectToAction("Details", "Electronics", new { id = ad.Id, title = ElectronicsController.URLFriendly(ad.title) });
                    }
                }
                return RedirectToAction("Register", "Account");
            }
            return View("Edit", ad);
        }

        public ActionResult Abc()
        {
            return View();
        }
        public ActionResult NewTemp()
        {
            return View();
        }
        public ActionResult Index()
        
        {
            var mobiles = from ad in db.MobileAds
                          where ad.Ad.AdImages.Count > 0
                          orderby ad.Ad.views
                          select new
                          {
                              id = ad.Ad.Id,
                              title = ad.Ad.title,
                          };
           // ViewBag.mobiles = mobiles;
            return View(mobiles);
        }
        //public string Index()
        //{
        //    if (subdomainName == null)
        //    {
        //        return "No subdomain";
        //    }
        //    return subdomainName;
        //}
        [Route("Search/{s?}")]
        public ActionResult search(string s = null)
        {
            ViewBag.search = s;
            return View();
        }

        public ActionResult Temp()
        {
            return View();
        }
        [Route("not-found")]
        public ActionResult notFound()
        {
            return View();
        }
        public ActionResult Temp4()
        {
            return View();
        }
        public ActionResult LandingPage()
        {
            return View();
        }
        public ActionResult temp2()
        {
            return View();
        }
        public ActionResult temp3(int id)
        {
            ViewBag.adId = id;
            return View();
        }
        
        //public ActionResult Index1(string category, string subcategory, string lowercategory,string lowercategory1,int id = 0, string ignore = "")
        //{
        //    var currentNode = this.GetCurrentSiteMapNode();
        //    if (currentNode != null)
        //    {
        //        switch (currentNode.Key)
        //        {
        //            case "titleid":
        //                currentNode.Title = ignore;
        //                currentNode.ParentNode.Title = lowercategory1;
        //                currentNode.ParentNode.ParentNode.Title = lowercategory;
        //                currentNode.ParentNode.ParentNode.ParentNode.Title = subcategory;
        //                currentNode.ParentNode.ParentNode.ParentNode.ParentNode.Title = category;
        //                break;
        //            case "lowercategory1":
        //                currentNode.Title = lowercategory1;
        //                currentNode.ParentNode.Title = lowercategory;
        //                currentNode.ParentNode.ParentNode.Title = subcategory;
        //                currentNode.ParentNode.ParentNode.ParentNode.Title = category;
        //                break;
        //            case "lowercategory":
        //                currentNode.Title = lowercategory;
        //                currentNode.ParentNode.Title = subcategory;
        //                currentNode.ParentNode.ParentNode.Title = category;
        //                break;
        //            case "subcategory":
        //                currentNode.Title = subcategory;
        //                currentNode.ParentNode.Title = category;
        //                break;
        //            case "category":
        //                currentNode.Title = category;
        //                break;
        //        }
        //    }
        //    if (category == null)
        //    {
        //        return View("landingPage");
        //    }
        //    else if (subcategory == null)
        //    {
        //        if (category == "create")
        //        {
        //            return View("createAd");
        //        }
        //        else if (category == "ask")
        //        {
        //            return View("../Forum/Create");
        //        }
        //        else if (category == "electronics")
        //        {
        //            return View("search");//electronics homepage or one page for all categories to select subcategory or all electronics featured ads
        //        }
        //        else 
        //        {
        //            return View("notFound");
        //        }
        //    }
        //    else if (lowercategory == null)
        //    {
        //        //if (category == "electronics")
        //        //{
        //        //    if (subcategory == "mobiles")
        //        //    {
        //        //        //return mobile ads
        //        //    }
        //        //}
        //        return View("search");
        //    }
        //    else if (lowercategory1 == null)
        //    {
        //        return View("search");
        //    }
        //    else if (id == null)
        //    {
        //        return View("search");
        //    }
        //    else
        //    {
        //        ViewBag.adId = id;
        //        return View("../Electronics/Details");
        //    }
        //    return View();
        //}
        [Route("CreateAd")]
        public ActionResult CreateAd(string category,string subcategory)
        {
            if (Request.IsAuthenticated)
            {
                if (category == null || subcategory == null)
                {
                    TempData["error"] = "Please mention both category and subcategory";
                    return View();
                }
                if (category == "Electronics")
                {
                    if (subcategory == "Mobiles")
                    {
                        return View("/Electronics/Create");
                    }
                }
            }
           // string path = Request.Url.AbsolutePath;
            if (Request.UrlReferrer == null)
            {
                return RedirectToAction("Index", "Home");
            }
            try
            {
                return View(Request.UrlReferrer.AbsolutePath);
            }
            catch (Exception e)
            {
                return RedirectToAction("Index","Home");
            }
            //string path = null;
            //if (Request.UrlReferrer.AbsolutePath != null)
            //{
            //    return View(Request.UrlReferrer.AbsolutePath);
            //}
                
        }
        public ActionResult About()
        {
            return View();
        }
      // [SiteMapTitle("title")]
        public ActionResult Contact(int id)
        {
            return View();
        }

        public ActionResult Minor()
        {
            ViewData["SubTitle"] = "Simple example of second view";
            ViewData["Message"] = "Data are passing to view by ViewData from controller";

            return View();
        }
    }
}