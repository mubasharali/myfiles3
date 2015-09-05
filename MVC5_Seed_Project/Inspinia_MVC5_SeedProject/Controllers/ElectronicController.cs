using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.AspNet.Identity;
using System.Web;
using Inspinia_MVC5_SeedProject.Models;

namespace Inspinia_MVC5_SeedProject.Controllers
{
    public class ElectronicController : ApiController
    {
        private Entities db = new Entities();

        // GET api/Electronic
        public IQueryable<Ad> GetAds()
        {
            return db.Ads;
        }
        //[Route("api/Electronic/{category}/{subcategory}/{lowcategory}/{title}")]
        public async Task<IHttpActionResult> search(string category, string subcategory, string lowcategory, string title)
        {
          //  IEnumerable<Ad> ads =await db.Ads.Where(x=>x.category == "Electronics").ToListAsync();
            var ret = (from ad in db.Ads
                      where ad.category == "Electronics" && ad.title.Contains(title)
                      orderby ad.time
                      select new
                      {
                          title = ad.title,
                          postedById = ad.AspNetUser.Id,
                          postedByName = ad.AspNetUser.UserName,
                          id = ad.Id,
                          price = ad.price,
                          isNegotiable = ad.isnegotiable,
                          mobilead = from mobile in ad.MobileAds.Where(x => x.Mobile.Id == subcategory && x.Mobile.MobileModels.Any(xu =>xu.model.Equals(lowcategory)) ).ToList()
                                     select new
                                     {
                                         color = mobile.color,
                                         condition = mobile.condition,
                                         company = mobile.Mobile.Id,
                                         sims = mobile.sims,
                                         model = mobile.Mobile.MobileModels.FirstOrDefault(x=>x.model.Equals(lowcategory)).model
                                     }
                      }).AsEnumerable();
            return Ok(ret);
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetMobileTree()
        {
            var mobiles = (from mobile in db.Mobiles.ToList()
                          orderby mobile.Id
                          select new
                          {
                              companyName = mobile.Id,
                              models = from model in mobile.MobileModels.ToList()
                                       select new
                                       {
                                           model = model.model
                                       }
                          }).AsEnumerable();
            return Ok(mobiles);
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetBrands()
        {
            var brands =  (db.Mobiles.Select(x=>x.Id)).AsEnumerable();
            return Ok(brands);
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetModels(string brand)
        {
            var models =await db.MobileModels.Where(x => x.Mobile == brand).Select(x => x.model).ToListAsync();
            return Ok(models);
        }
        // GET api/Electronic/5
        [ResponseType(typeof(Ad))]
        public async Task<IHttpActionResult> GetAd(int id)
        {
            Ad add = await db.Ads.FindAsync(id);
            if (add == null)
            {
                return NotFound();
            }
            await AdViews(id);
            string islogin = "";
            if (User.Identity.IsAuthenticated)
            {
                islogin = User.Identity.GetUserId();
            }
            var ret = (from ad in db.Ads
                       where ad.Id == id
                       orderby ad.time
                       select new
                       {
                           title = ad.title,
                           postedById = ad.AspNetUser.Id,
                           postedByName = ad.AspNetUser.UserName,
                           description = ad.description,
                           id = ad.Id,
                           time = ad.time,
                           islogin = islogin,
                           isNegotiable = ad.isnegotiable,
                           price = ad.price,
                           reportedCount = ad.Reporteds.Count,
                           isReported = ad.Reporteds.Any(x => x.reportedBy == islogin),
                           views = ad.AdViews.Count,
                           bid = from biding in ad.Bids.ToList()
                                 select new
                                 {
                                     postedByName = biding.AspNetUser.UserName,
                                     postedById = biding.AspNetUser.Id,
                                     price = biding.price,
                                     time = biding.time,
                                     id = biding.Id,
                                 },
                           mobilead = from mobile in ad.MobileAds.ToList()
                                      select new
                                      {
                                          color = mobile.color,
                                          condition = mobile.condition,
                                          sims = mobile.sims,
                                      },
                           comment = from comment in ad.Comments.ToList()
                                     orderby comment.time
                                     select new
                                     {
                                         description = comment.description,
                                         postedById = comment.postedBy,
                                         postedByName = comment.AspNetUser.UserName,
                                         time = comment.time,
                                         id = comment.Id,
                                         adId = comment.adId,
                                         islogin = islogin,
                                         voteUpCount = comment.CommentVotes.Where(x => x.isup == true).Count(),
                                         voteDownCount = comment.CommentVotes.Where(x => x.isup == false).Count(),
                                         isUp = comment.CommentVotes.Any(x => x.votedBy == islogin && x.isup),
                                         isDown = comment.CommentVotes.Any(x => x.votedBy == islogin && x.isup == false),
                                         commentReply = from commentreply in comment.CommentReplies.ToList()
                                                        orderby commentreply.time
                                                        select new
                                                        {
                                                            id = commentreply.Id,
                                                            description = commentreply.description,
                                                            postedById = commentreply.postedBy,
                                                            postedByName = commentreply.AspNetUser.UserName,
                                                            time = commentreply.time,
                                                            voteUpCount = commentreply.CommentReplyVotes.Where(x => x.isup == true).Count(),
                                                            voteDownCount = commentreply.CommentReplyVotes.Where(x => x.isup == false).Count(),
                                                            isUp = commentreply.CommentReplyVotes.Any(x => x.votedBy == islogin && x.isup),
                                                            isDown = commentreply.CommentReplyVotes.Any(x => x.votedBy == islogin && x.isup == false)
                                                        }
                                     }
                       }).AsEnumerable();
            return Ok(ret);
        }
        
        [HttpPost]
        public async Task<IHttpActionResult> postBid(Bid bid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!(HttpContext.Current.Request.IsAuthenticated))
            {
                return BadRequest();
                //return BadRequest("you must be logged in post comment");
            }
            bid.time = DateTime.UtcNow;
            bid.postedBy = User.Identity.GetUserId();
            db.Bids.Add(bid);
            await db.SaveChangesAsync();
            var ret = db.Bids.Where(x => x.Id == bid.Id).Select(x => new
            {
                price = x.price,
                postedById = x.postedBy,
                postedByName = x.AspNetUser.UserName,
                time = x.time,
                id = x.Id,
            }).FirstOrDefault();
            return Ok(ret);
            // return CreatedAtRoute("DefaultApi", new { id = comment.Id }, comment);
        }
        [HttpPost]
        public async Task<IHttpActionResult> UpdateBid(Bid bid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            db.Entry(bid).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            return StatusCode(HttpStatusCode.NoContent);
        }
        [HttpPost]
        public async Task<IHttpActionResult> DeleteBid(int id)
        {
            if (HttpContext.Current.Request.IsAuthenticated)
            {
                Bid bid = await db.Bids.FindAsync(id);
                if (bid == null)
                {
                    return NotFound();
                }
                db.Bids.Remove(bid);
                await db.SaveChangesAsync();
                return Ok(bid);
            }
            return BadRequest();
        }
        public async Task<IHttpActionResult> PutAd(int id, Ad ad)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != ad.Id)
            {
                return BadRequest();
            }

            db.Entry(ad).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST api/Electronic
        [ResponseType(typeof(Ad))]
        public async Task<IHttpActionResult> PostAd(Ad ad)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Ads.Add(ad);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = ad.Id }, ad);
        }

        // DELETE api/Electronic/5
        [HttpPost]
        public async Task<IHttpActionResult> DeleteAd(int id)
        {
            Ad ad = await db.Ads.FindAsync(id);
            if (ad == null)
            {
                return NotFound();
            }

            db.Ads.Remove(ad);
            
            await db.SaveChangesAsync();
            
            return Ok(ad);
        }
        protected string GetIPAddress()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }
        public async Task<IHttpActionResult> AdViews(int id)
        {
            Ad ad = await db.Ads.FindAsync(id);
            if (ad == null)
            {
                return NotFound();
            }
            var userId = User.Identity.GetUserId();
            if (userId != null)
            {
                var isAlreadyViewed = ad.AdViews.Any(x => x.viewedBy == userId);
                if (isAlreadyViewed)
                {
                    return Ok();
                }
                AdView rep = new AdView();
                rep.viewedBy = userId;
                rep.adId = id;
                db.AdViews.Add(rep);
                await db.SaveChangesAsync();

                return Ok();
            }
            else
            {
                string ip = GetIPAddress();
                var isAlreadyViewed = ad.AdViews.Any(x => x.viewedBy == ip);
                if (isAlreadyViewed)
                {
                    return Ok();
                }
                AdView rep = new AdView();
                rep.viewedBy = ip;
                rep.adId = id;
                db.AdViews.Add(rep);
                await db.SaveChangesAsync();
                return Ok();
            }

        }
        public async Task<IHttpActionResult> ReportAd(int id)
        {
            var userId = User.Identity.GetUserId();
            if(userId != null)
            {
                Ad ad = await db.Ads.FindAsync(id);
                if (ad == null)
                {
                    return NotFound();
                }
                var isAlreadyReported = ad.Reporteds.Any(x => x.reportedBy == userId);
                if (isAlreadyReported)
                {
                    return BadRequest("You can report an Ad only once.If something really wrong you can contact us");
                }
                Reported rep = new Reported();
                rep.reportedBy = userId;
                rep.adId = id;
                db.Reporteds.Add(rep);
                await db.SaveChangesAsync();

                var count = ad.Reporteds.Count;

                return Ok(count);
            }
            else{
                return BadRequest("Not login");
            }
            
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AdExists(int id)
        {
            return db.Ads.Count(e => e.Id == id) > 0;
        }
    }
}