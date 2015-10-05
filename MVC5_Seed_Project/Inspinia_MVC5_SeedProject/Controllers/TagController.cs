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
using Inspinia_MVC5_SeedProject.Models;
using Microsoft.AspNet.Identity;
namespace Inspinia_MVC5_SeedProject.Controllers
{
    public class TagController : ApiController
    {
        private Entities db = new Entities();

        // GET api/Tag
        public async Task<IHttpActionResult> GetTag(int id)
        {
            string loginUserId = "";
            if (User.Identity.IsAuthenticated)
            {
                loginUserId = User.Identity.GetUserId();
            }
            var ret = await (from tag in db.Tags
                      where tag.Id.Equals(id)
                      select new
                      {
                          id = tag.Id,
                          createdById = tag.createdBy,
                          createdByName = tag.AspNetUser.Email,
                          updatedById = tag.updatedBy,
                          updatedByName = tag.AspNetUser1.Email,
                          updatedTime = tag.updatedTime,
                          info = tag.info,
                          time = tag.time,
                          followers = tag.FollowTags.Count,
                          isFollowed = tag.FollowTags.Any(x => x.followedBy == loginUserId),
                          loginUserId = loginUserId,
                          name = tag.name,
                          isReported = tag.ReportedTags.Any(x=>x.reportedBy.Equals(loginUserId)),
                          reportedCount = tag.ReportedTags.Count,
                          ads = from ad in tag.AdTags
                                where ad.tagId.Equals(id)
                                select new
                                {
                                    title = ad.Ad.title,
                                    id = ad.Ad.Id,
                                    //other s
                                },
                          questions = from question in tag.QuestionTags
                                      where question.tagId.Equals(id)
                                      select new
                                      {
                                          title = question.Question.title,
                                          id = question.Question.Id,
                                          views = question.Question.QuestionViews.Count(),
                                          answers = question.Question.Answers.Count(),
                                          questionVoteUpCount = question.Question.QuestionVotes.Count,
                                          questionVoteDownCount = question.Question.QuestionVotes.Count(x=>x.isUp == false),
                                          time = question.Question.time,
                                          //others
                                      }
                      }).FirstOrDefaultAsync();
            return Ok(ret);
        }
        [HttpPost]
        public async Task<IHttpActionResult> UpdateTag(Tag id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
           // db.Entry(tag).State = EntityState.Modified;
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
        public async Task<IHttpActionResult> DeleteTag(int id)
        {
            Tag comment = await db.Tags.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            db.Tags.Remove(comment);
            await db.SaveChangesAsync();

            return Ok(comment);
        }
        public async Task<IHttpActionResult> RecentAddedTags(int daysago)
        {
            TimeSpan duration = DateTime.UtcNow - DateTime.Today.AddDays(-daysago);
            DateTime days = DateTime.UtcNow - duration;
            var ret = from tag in db.Tags
                      where tag.time >= days
                      select new
                      {
                          createdBy = tag.createdBy,
                          createdByName = tag.AspNetUser.UserName,
                          name = tag.name,
                          info = tag.info,
                          time = tag.time,
                          id = tag.Id,
                          updatedTime = tag.updatedTime,
                          updatedBy = tag.updatedBy,
                      };
            return Ok(ret);
        }
        public async Task<IHttpActionResult> RecentUpdatedTags(int daysago)
        {
            TimeSpan duration = DateTime.UtcNow - DateTime.Today.AddDays(-daysago);
            DateTime days = DateTime.UtcNow - duration;
            var ret = from tag in db.Tags
                      where tag.updatedTime >= days
                      select new
                      {
                          createdBy = tag.createdBy,
                          createdByName = tag.AspNetUser.UserName,
                          name = tag.name,
                          info = tag.info,
                          time = tag.time,
                          id = tag.Id,
                          updatedTime = tag.updatedTime,
                          updatedBy = tag.updatedBy,
                      };
            return Ok(ret);
        }
        public async Task<IHttpActionResult> ReportedTags(int daysago)
        {
            TimeSpan duration = DateTime.UtcNow - DateTime.Today.AddDays(-daysago);
            DateTime days = DateTime.UtcNow - duration;
            var ret = from tag in db.Tags
                      //where tag.time >= days
                      where tag.ReportedTags.Count > 0
                      orderby tag.ReportedTags.Count
                      select new
                      {
                          createdBy = tag.createdBy,
                          createdByName = tag.AspNetUser.UserName,
                          name = tag.name,
                          info = tag.info,
                          time = tag.time,
                          id = tag.Id,
                          updatedTime = tag.updatedTime,
                          updatedBy = tag.updatedBy,
                      };
            return Ok(ret);
        }
        public async Task<IHttpActionResult> SearchTags(string s)
        {
            //var ret = db.Tags.Where(x => x.name.Contains(s));
            var ret = from tag in db.Tags
                      where tag.name.Contains(s)
                      select new
                      {
                          id = tag.Id,
                          info = tag.info,
                          name = tag.name,
                          followers = tag.FollowTags.Count()
                      };
            return Ok(ret);
        }
        public async Task<IHttpActionResult> Follow(int tagId)
        {
            var userId = User.Identity.GetUserId();
            if (userId != null)
            {
                string s = "";
                int c = 0;
                var data = new { status = s, count = c };
                Tag q = await db.Tags.FindAsync(tagId);
                var followed = q.FollowTags.FirstOrDefault(x => x.followedBy == userId);
                if (followed != null)
                {
                    db.FollowTags.Remove(followed);
                    db.SaveChanges();
                    s = "Follow";
                    c = await db.FollowTags.CountAsync(x=>x.tagId.Equals(tagId));
                    data = new { status = s, count = c };
                    return Ok(data);
                }
                FollowTag f = new  FollowTag();
                f.followedBy = userId;
                f.tagId = tagId;
                db.FollowTags.Add(f);
                db.SaveChanges();
                s = "UnFollow";
                c = await db.FollowTags.CountAsync(x=>x.tagId.Equals(tagId));
                data = new { status = s, count = c };
                return Ok(data);
            }
            else
            {
                return BadRequest("You are not login");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> ReportTag(int id)
        {
            var userId = User.Identity.GetUserId();
            if (userId != null)
            {
                Tag ad = await db.Tags.FindAsync(id);
                if (ad == null)
                {
                    return NotFound();
                }
                var isAlreadyReported = ad.ReportedTags.Any(x => x.reportedBy == userId);
                if (isAlreadyReported)
                {
                    return BadRequest("You can report a Tag only once.If something really wrong you can contact us");
                }
                ReportedTag rep = new  ReportedTag();
                rep.reportedBy = userId;
                rep.tagId = id;
                db.ReportedTags.Add(rep);
                await db.SaveChangesAsync();

                var count = ad.ReportedTags.Count;

                return Ok(count);
            }
            else
            {
                return BadRequest("Not login");
            }

        }
        [HttpPost]
        public async Task<IHttpActionResult> updateTag(Tag comment)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                comment.updatedBy = null;
                comment.updatedTime = null;
                comment.updatedTime = DateTime.UtcNow;
                comment.updatedBy = User.Identity.GetUserId();
                db.Entry(comment).State = EntityState.Modified;

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    string s = e.ToString();
                    return BadRequest("Exception");
                }
                var data = new
                {
                    updatedById = comment.updatedBy,
                    updatedByName =db.AspNetUsers.Find(comment.updatedBy).Email,
                    updatedTime = comment.updatedTime
                };
                return Ok(data);
            }
            return BadRequest("Not login");
        }
        // GET api/Tag/5
        //[ResponseType(typeof(Tag))]
        //public async Task<IHttpActionResult> GetTag(int id)
        //{
        //    Tag tag = await db.Tags.FindAsync(id);
        //    if (tag == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(tag);
        //}

        // PUT api/Tag/5
        public async Task<IHttpActionResult> PutTag(int id, Tag tag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tag.Id)
            {
                return BadRequest();
            }

            db.Entry(tag).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TagExists(id))
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

        // POST api/Tag
        [ResponseType(typeof(Tag))]
        public async Task<IHttpActionResult> PostTag(Tag tag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Tags.Add(tag);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = tag.Id }, tag);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TagExists(int id)
        {
            return db.Tags.Count(e => e.Id == id) > 0;
        }
    }
}