using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Net.Http.Formatting;
using Microsoft.AspNet.Identity;
using Inspinia_MVC5_SeedProject.Models;

namespace Inspinia_MVC5_SeedProject.Controllers
{
    public class CompanyController : ApiController
    {
        private Entities db = new Entities();

        // GET api/Company
        public IQueryable<Company> GetCompanies()
        {
            return db.Companies;
        }

        // GET api/Company/5
        [ResponseType(typeof(Company))]
        public async Task<IHttpActionResult> GetCompany(int id)
        {
            Company company = await db.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }

            return Ok(company);
        }
        public async Task<IHttpActionResult> AddReview(Review review)
        {
            if (User.Identity.IsAuthenticated)
            {
                review.reviewedBy = User.Identity.GetUserId();
                db.Reviews.Add(review);
                await db.SaveChangesAsync();
                return Ok("Done");
            }
            return BadRequest();
        }
        public async Task<IHttpActionResult> UpdateReview(Review review)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                db.Entry(review).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Ok("Done");
            }
            return BadRequest();
        }
        public async Task<IHttpActionResult> DeleteReview(Review review)
        {
            if (User.Identity.IsAuthenticated)
            {
                db.Reviews.Remove(review);
                await db.SaveChangesAsync();
                return Ok("Done");
            }
            return BadRequest();
        }
        public async Task<IHttpActionResult> GetPage(int id)
        {
            var loginUserId = User.Identity.GetUserId();
            var loginUserProfileExtension = db.AspNetUsers.Find(loginUserId).dpExtension;
            var ret = from company in db.Companies
                      where company.Id.Equals(id)
                      select new
                      {
                          rating = from review in company.Reviews
                                   //where review.reviewedBy.Equals(loginUserId)
                                   select new
                                   {
                                       id = review.Id,
                                       rating = review.rating,
                                       reviewDescription = review.description,
                                       time = review.time,
                                       reviewedBy = review.reviewedBy,
                                       reviewedByName = review.AspNetUser.Email,
                                   },
                          loginUserId = loginUserId,
                          loginUserProfileExtension = loginUserProfileExtension,
                          id = company.Id,
                          title = company.title,
                          time = company.time,
                          since = company.since,
                          shortAbout = company.shortabout,
                          longAbout = company.longabout,
                          logoExtension = company.logoextension,
                          owner = company.owner,
                          status = company.status,
                          category = company.category,
                          twlink = company.twlink,
                          fblink = company.fblink,
                          contactNo1 = company.contactNo1,
                          contactNo2 = company.contactNo2,
                          email = company.email,
                          websiteLink = company.websitelink,
                          createdById = company.AspNetUser.Id,
                          createdByName = company.AspNetUser.Email,
                          cityName = company.City.cityName,
                          cityId = (int?)company.City.Id,
                          popularPlaceId = (int?)company.popularPlaceId,
                          popularPlaceName = company.popularPlace.name,
                          exectLocation = company.exectLocation,
                          branches = from branch in company.CompanyOffices.ToList()
                                     select new
                                     {
                                         id = branch.Id,
                                         since = branch.since,
                                         cityId = branch.cityId,
                                         cityName = branch.City.cityName,
                                         popularPlaceId = branch.popularPlaceId,
                                         popularPlace = branch.popularPlace.name,
                                         exectLocation = branch.exectLocation,
                                         contactNo1 = branch.contactNo1,
                                         contactNo2 = branch.contactNo2
                                     },
                          tags = company.CompanyTags.Select(x => x.Tag.name)

                      };

            return Ok(ret);
        }
        [HttpPost]
        public async Task<IHttpActionResult> HeadOfficeLocation(Company branch, string city, string popularPlace, string exectLocation)
        {
           // var branch = await db.Companies.FindAsync(companyId);
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
                    await db.SaveChangesAsync();
                    // loc.cityId = cit.Id;
                    branch.cityId = cit.Id;
                    if (popularPlace != null)
                    {
                        popularPlace pop = new popularPlace();
                        pop.cityId = cit.Id;
                        pop.name = popularPlace;
                        pop.addedBy = User.Identity.GetUserId();
                        pop.addedOn = DateTime.UtcNow;
                        db.popularPlaces.Add(pop);
                        await db.SaveChangesAsync();
                        //  loc.popularPlaceId = pop.Id;
                        branch.popularPlaceId = pop.Id;
                    }
                }
                else
                {
                    // loc.cityId = citydb.Id;
                    branch.cityId = citydb.Id;
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
                            await db.SaveChangesAsync();
                            //   loc.popularPlaceId = pop.Id;
                            branch.popularPlaceId = pop.Id;
                        }
                        else
                        {
                            //   loc.popularPlaceId = ppp.Id;
                            branch.popularPlaceId = ppp.Id;
                        }
                    }
                }
                branch.exectLocation = exectLocation;
                
                try
                {
                    db.Entry(branch).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    string s = e.ToString();
                }
            }
            //var ret = await (from br in db.CompanyOffices
            //                 where br.Id.Equals(branch.Id)
            //                 select new
            //                 {
            //                     id = br.Id,
            //                     cityId = br.cityId,
            //                     cityName = br.City.cityName,
            //                     popularPlace = br.popularPlace.name,
            //                     popularPlaceId = br.popularPlaceId,
            //                     contactNo1 = br.contactNo1,
            //                     contactNo2 = br.contactNo2,
            //                     since = br.since,
            //                     exectLocation = br.exectLocation,
            //                 }).FirstOrDefaultAsync();
            return Ok("Done");
        }

        public async Task<IHttpActionResult> OfficeBranch(CompanyOffice branch, string city, string popularPlace, string exectLocation,string SaveOrUpdate)
        {
            //var company = await db.Companies.FindAsync(companyId);
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
                    await db.SaveChangesAsync();
                    // loc.cityId = cit.Id;
                    branch.cityId = cit.Id;
                    if (popularPlace != null)
                    {
                        popularPlace pop = new popularPlace();
                        pop.cityId = cit.Id;
                        pop.name = popularPlace;
                        pop.addedBy = User.Identity.GetUserId();
                        pop.addedOn = DateTime.UtcNow;
                        db.popularPlaces.Add(pop);
                        await db.SaveChangesAsync();
                        //  loc.popularPlaceId = pop.Id;
                        branch.popularPlaceId = pop.Id;
                    }
                }
                else
                {
                    // loc.cityId = citydb.Id;
                    branch.cityId = citydb.Id;
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
                            await db.SaveChangesAsync();
                            //   loc.popularPlaceId = pop.Id;
                            branch.popularPlaceId = pop.Id;
                        }
                        else
                        {
                            //   loc.popularPlaceId = ppp.Id;
                            branch.popularPlaceId = ppp.Id;
                        }
                    }
                }
                branch.exectLocation = exectLocation;
                if (SaveOrUpdate == "Save")
                {
                    db.CompanyOffices.Add(branch);
                }
                else if (SaveOrUpdate == "Update")
                {
                    //var bra = db.CompanyOffices.Find(branch.Id);
                    //bra.popularPlaceId = branch.popularPlaceId;
                    //bra.since = branch.since;
                    //bra.cityId = branch.cityId;
                    //bra.contactNo1 = branch.contactNo1;
                    //bra.contactNo2 = branch.contactNo2;
                    //await db.SaveChangesAsync();
                    db.Entry(branch).State = EntityState.Modified;
                }
                try
                {
                    await db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    string s = e.ToString();
                }
            }
            var ret = await (from br in db.CompanyOffices
                      where br.Id.Equals(branch.Id)
                      select new
                      {
                          id = br.Id,
                          cityId = br.cityId,
                          cityName = br.City.cityName,
                          popularPlace = br.popularPlace.name,
                          popularPlaceId = br.popularPlaceId,
                          contactNo1 = br.contactNo1,
                          contactNo2 = br.contactNo2,
                          since = br.since,
                          exectLocation = br.exectLocation,
                      }).FirstOrDefaultAsync();
            return Ok(ret);
        }
        [HttpPost]
        public async Task<IHttpActionResult> DeleteBranch(int id)
        {
            CompanyOffice company = await db.CompanyOffices.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }

            db.CompanyOffices.Remove(company);
            await db.SaveChangesAsync();

            return Ok("Done");
        }
        public async Task<IHttpActionResult> UpdatePage(Company comment)
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
            
            catch (DbEntityValidationException e)
            {
                string s = e.ToString();
                List<string> errorMessages = new List<string>();
                foreach (DbEntityValidationResult validationResult in e.EntityValidationErrors)
                {
                    string entityName = validationResult.Entry.Entity.GetType().Name;
                    foreach (DbValidationError error in validationResult.ValidationErrors)
                    {
                        errorMessages.Add(entityName + "." + error.PropertyName + ": " + error.ErrorMessage);
                    }
                }
            }
            catch (Exception e)
            {
                string s = e.ToString();
            }
            return StatusCode(HttpStatusCode.NoContent);
        }
        public class BranchLocation1
        {
            public string cityName;
            public string popularPlace;
        }
        
        // DELETE api/Company/5
        [ResponseType(typeof(Company))]
        public async Task<IHttpActionResult> DeleteCompany(int id)
        {
            Company company = await db.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }

            db.Companies.Remove(company);
            await db.SaveChangesAsync();

            return Ok(company);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CompanyExists(int id)
        {
            return db.Companies.Count(e => e.Id == id) > 0;
        }
    }
}