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
using Inspinia_MVC5_SeedProject.Models;

namespace Inspinia_MVC5_SeedProject.Controllers
{
    public class ForumController : ApiController
    {
        private Entities db = new Entities();

        // GET api/Forum
        public IQueryable<Question> GetQuestions()
        {
            return db.Questions;
        }

        // GET api/Forum/5
        [ResponseType(typeof(Question))]
        public async Task<IHttpActionResult> GetQuestion(int id)
        {
            Question question = await db.Questions.FindAsync(id);
            if (question == null)
            {
                return NotFound();
            }
            await QuestionViews(id);
            string islogin = "";
            if (User.Identity.IsAuthenticated)
            {
                islogin = User.Identity.GetUserId();
            }
            var ret = from q in db.Questions
                      where q.Id == id
                      select new
                      {
                          title = q.title,
                          description = q.description,
                          id = q.Id,
                          postedById = q.AspNetUser.Id,
                          postedByName = q.AspNetUser.UserName,
                          time = q.time,
                          islogin = islogin,
                          views = q.QuestionViews.Count,
                          reportedCount = q.ReportedQuestions.Count,
                          isReported = q.ReportedQuestions.Any(x=>x.reportedBy == islogin),
                          questionReplies = from reply in q.QuestionReplies.ToList()
                                            select new
                                            {
                                                id = reply.Id,
                                                description = reply.description,
                                                postedById = reply.AspNetUser.Id,
                                                postedByName = reply.AspNetUser.UserName,
                                                time = reply.time
                                            },
                          answers = from ans in q.Answers.ToList()
                                    select new
                                    {
                                        id = ans.Id,
                                        description = ans.description,
                                        postedByName = ans.AspNetUser.UserName,
                                        postedById = ans.AspNetUser.Id,
                                        time = ans.time,
                                        answerReplies = from rep in ans.AnswerReplies.ToList()
                                                        select new
                                                        {
                                                            id = rep.Id,
                                                            description = rep.description,
                                                            postedByName = rep.AspNetUser.UserName,
                                                            postedById = rep.AspNetUser.Id,
                                                            time = rep.time
                                                        }
                                    }
                      };
            return Ok(ret);
        }

        public async Task<IHttpActionResult> QuestionViews(int id)
        {
            Question ad = await db.Questions.FindAsync(id);
            if (ad == null)
            {
                return NotFound();
            }
            var userId = User.Identity.GetUserId();
            if (userId != null)
            {
                var isAlreadyViewed = ad.QuestionViews.Any(x => x.viewedBy == userId);
                if (isAlreadyViewed)
                {
                    return Ok();
                }
                QuestionView rep = new QuestionView();
                rep.viewedBy = userId;
                rep.questionId = id;
                db.QuestionViews.Add(rep);
                await db.SaveChangesAsync();

                return Ok();
            }
            else
            {
                string ip = GetIPAddress();
                var isAlreadyViewed = ad.QuestionViews.Any(x => x.viewedBy == ip);
                if (isAlreadyViewed)
                {
                    return Ok();
                }
                QuestionView rep = new QuestionView();
                rep.viewedBy = ip;
                rep.questionId = id;
                db.QuestionViews.Add(rep);
                await db.SaveChangesAsync();
                return Ok();
            }

        }
        // PUT api/Forum/5
        public async Task<IHttpActionResult> PutQuestion(int id, Question question)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != question.Id)
            {
                return BadRequest();
            }

            db.Entry(question).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuestionExists(id))
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

        // POST api/Forum
        [ResponseType(typeof(Question))]
        public async Task<IHttpActionResult> PostQuestion(Question question)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            question.time = DateTime.UtcNow;
            question.postedBy = User.Identity.GetUserId();
            db.Questions.Add(question);
            await db.SaveChangesAsync();
            return CreatedAtRoute("DefaultApi", new { id = question.Id }, question);
        }

        // DELETE api/Forum/5
        [ResponseType(typeof(Question))]
        public async Task<IHttpActionResult> DeleteQuestion(int id)
        {
            Question question = await db.Questions.FindAsync(id);
            if (question == null)
            {
                return NotFound();
            }

            db.Questions.Remove(question);
            await db.SaveChangesAsync();

            return Ok(question);
        }
        [HttpPost]
        public async Task<IHttpActionResult> DeleteAnswer(int id)
        {
            Answer comment = await db.Answers.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            db.Answers.Remove(comment);
            await db.SaveChangesAsync();
            return Ok(comment);
        }
        [HttpPost]
        public async Task<IHttpActionResult> DeleteQuestionReply(int id)
        {
            QuestionReply comment = await db.QuestionReplies.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            db.QuestionReplies.Remove(comment);
            await db.SaveChangesAsync();

            return Ok(comment);
        }
        [HttpPost]
        public async Task<IHttpActionResult> DeleteAnswerReply(int id)
        {
            AnswerReply comment = await db.AnswerReplies.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            db.AnswerReplies.Remove(comment);
            await db.SaveChangesAsync();

            return Ok(comment);
        }
        public async Task<IHttpActionResult> PostQuestionReply(QuestionReply question)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid model state");
                }
                question.time = DateTime.UtcNow;
                question.postedBy = User.Identity.GetUserId();
                db.QuestionReplies.Add(question);
                await db.SaveChangesAsync();
                var ret = db.QuestionReplies.Where(x => x.Id == question.Id).Select(x => new
                {
                    id = x.Id,
                    description = x.description,
                    postedById = x.AspNetUser.Id,
                    postedByName = x.AspNetUser.UserName,
                    time = x.time
                }).FirstOrDefault();
                return Ok(ret);
            }
            return BadRequest("Not login");
        }
        [HttpPost]
        public async Task<IHttpActionResult> updateQuestionReply(QuestionReply comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            db.Entry(comment).State = EntityState.Modified;

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

        public async Task<IHttpActionResult> PostAnswerReply(AnswerReply question)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid model state");
                }
                question.time = DateTime.UtcNow;
                question.postedBy = User.Identity.GetUserId();
                db.AnswerReplies.Add(question);
                await db.SaveChangesAsync();
                var ret =await db.AnswerReplies.Where(x => x.Id == question.Id).Select(x => new
                {
                    id = x.Id,
                    description = x.description,
                    postedById = x.AspNetUser.Id,
                    postedByName = x.AspNetUser.UserName,
                    time = x.time
                }).FirstOrDefaultAsync();
                return Ok(ret);
            }
            return BadRequest("Not login");
        }
        [HttpPost]
        public async Task<IHttpActionResult> updateAnswerReply(AnswerReply comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            db.Entry(comment).State = EntityState.Modified;

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
        public async Task<IHttpActionResult> PostAnswer(Answer question)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid model state");
                }
                question.time = DateTime.UtcNow;
                question.postedBy = User.Identity.GetUserId();
                db.Answers.Add(question);
                await db.SaveChangesAsync();
                var ret = db.Answers.Where(x => x.Id == question.Id).Select(x => new
                {
                    id = x.Id,
                    description = x.description,
                    postedById = x.AspNetUser.Id,
                    postedByName = x.AspNetUser.UserName,
                    time = x.time
                }).FirstOrDefault();
                return Ok(ret);
            }
            else
            {
                return BadRequest("Not login");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> updateAnswer(Answer comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            db.Entry(comment).State = EntityState.Modified;

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
        public async Task<IHttpActionResult> ReportQuestion(int id)
        {
            var userId = User.Identity.GetUserId();
            if (userId != null)
            {
                Question ad = await db.Questions.FindAsync(id);
                if (ad == null)
                {
                    return NotFound();
                }
                var isAlreadyReported = ad.ReportedQuestions.Any(x => x.reportedBy == userId);
                if (isAlreadyReported)
                {
                    return BadRequest("You can report a Question only once.If something really wrong you can contact us");
                }
                ReportedQuestion rep = new ReportedQuestion();
                rep.reportedBy = userId;
                rep.questionId = id;
                db.ReportedQuestions.Add(rep);
                await db.SaveChangesAsync();

                var count = ad.ReportedQuestions.Count;

                return Ok(count);
            }
            else
            {
                return BadRequest("Not login");
            }
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
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool QuestionExists(int id)
        {
            return db.Questions.Count(e => e.Id == id) > 0;
        }
    }
}