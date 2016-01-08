﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcSiteMapProvider.Web.Mvc;
using Inspinia_MVC5_SeedProject.Models;
namespace Inspinia_MVC5_SeedProject.Controllers
{
  //  [InitializeSimpleMembership]
    public class HomeController : Controller
    {
        public Entities db = new Entities();
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
        //public ActionResult Index()
        //{
        //    if (this.subdomainName != null)
        //    {
        //        return RedirectToAction("Index", this.subdomainName);
        //    }
        //    //db.Ads.Join(db.Comments,x=>x.Id,x=>x.adId,(a,c)=>new{a.});
            
        //    return View();
        //}
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
            ViewBag.mobiles = mobiles;
            return View();
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
        
        public ActionResult Index1(string category, string subcategory, string lowercategory,string lowercategory1,int id = 0, string ignore = "")
        {
            var currentNode = this.GetCurrentSiteMapNode();
            if (currentNode != null)
            {
                switch (currentNode.Key)
                {
                    case "titleid":
                        currentNode.Title = ignore;
                        currentNode.ParentNode.Title = lowercategory1;
                        currentNode.ParentNode.ParentNode.Title = lowercategory;
                        currentNode.ParentNode.ParentNode.ParentNode.Title = subcategory;
                        currentNode.ParentNode.ParentNode.ParentNode.ParentNode.Title = category;
                        break;
                    case "lowercategory1":
                        currentNode.Title = lowercategory1;
                        currentNode.ParentNode.Title = lowercategory;
                        currentNode.ParentNode.ParentNode.Title = subcategory;
                        currentNode.ParentNode.ParentNode.ParentNode.Title = category;
                        break;
                    case "lowercategory":
                        currentNode.Title = lowercategory;
                        currentNode.ParentNode.Title = subcategory;
                        currentNode.ParentNode.ParentNode.Title = category;
                        break;
                    case "subcategory":
                        currentNode.Title = subcategory;
                        currentNode.ParentNode.Title = category;
                        break;
                    case "category":
                        currentNode.Title = category;
                        break;
                }
            }
            if (category == null)
            {
                return View("landingPage");
            }
            else if (subcategory == null)
            {
                if (category == "create")
                {
                    return View("createAd");
                }
                else if (category == "ask")
                {
                    return View("../Forum/Create");
                }
                else if (category == "electronics")
                {
                    return View("search");//electronics homepage or one page for all categories to select subcategory or all electronics featured ads
                }
                else 
                {
                    return View("notFound");
                }
            }
            else if (lowercategory == null)
            {
                //if (category == "electronics")
                //{
                //    if (subcategory == "mobiles")
                //    {
                //        //return mobile ads
                //    }
                //}
                return View("search");
            }
            else if (lowercategory1 == null)
            {
                return View("search");
            }
            else if (id == null)
            {
                return View("search");
            }
            else
            {
                ViewBag.adId = id;
                return View("../Electronics/Details");
            }
            return View();
        }
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