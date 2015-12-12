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
        public async Task<IHttpActionResult> GetPage(int id)
        {
            var loginUserId = User.Identity.GetUserId();
            var loginUserProfileExtension = db.AspNetUsers.Find(loginUserId).dpExtension;
            var ret = from company in db.Companies
                      where company.Id.Equals(id)
                      select new
                      {
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
                          cityId =(int?) company.City.Id,
                          popularPlaceId = (int?)company.popularPlaceId,
                          popularPlaceName = company.popularPlace.name,
                          exectLocation = company.exectLocation,
                      };

            return Ok(ret);
        }
        //public async Task<IHttpActionResult> UpdateLocation(string city, string popularPlace, string exectLocation)
        //{
        //    if (city != null)
        //    {
        //        var citydb = db.Cities.FirstOrDefault(x => x.cityName.Equals(city, StringComparison.OrdinalIgnoreCase));
        //        if (citydb == null)
        //        {
        //            City cit = new City();
        //            cit.cityName = city;
        //            cit.addedBy = User.Identity.GetUserId();
        //            cit.addedBy = User.Identity.GetUserId();
        //            cit.addedOn = DateTime.UtcNow;
        //            db.Cities.Add(cit);
        //            db.SaveChanges();
        //            // loc.cityId = cit.Id;
        //            if (popularPlace != null)
        //            {
        //                popularPlace pop = new popularPlace();
        //                pop.cityId = cit.Id;
        //                pop.name = popularPlace;
        //                pop.addedBy = User.Identity.GetUserId();
        //                pop.addedOn = DateTime.UtcNow;
        //                db.popularPlaces.Add(pop);
        //                db.SaveChanges();
        //                //  loc.popularPlaceId = pop.Id;
        //            }
        //        }
        //        else
        //        {
        //            // loc.cityId = citydb.Id;
        //            if (popularPlace != null)
        //            {
        //                var ppp = db.popularPlaces.FirstOrDefault(x => x.City.cityName.Equals(city, StringComparison.OrdinalIgnoreCase) && x.name.Equals(popularPlace, StringComparison.OrdinalIgnoreCase));
        //                if (ppp == null)
        //                {
        //                    popularPlace pop = new popularPlace();
        //                    pop.cityId = citydb.Id;
        //                    pop.name = popularPlace;
        //                    pop.addedBy = User.Identity.GetUserId();
        //                    pop.addedOn = DateTime.UtcNow;
        //                    db.popularPlaces.Add(pop);
        //                    db.SaveChanges();
        //                    //   loc.popularPlaceId = pop.Id;
        //                }
        //                else
        //                {
        //                    //   loc.popularPlaceId = ppp.Id;
        //                }
        //            }
        //        }
        //        //  loc.exectLocation = exectLocation;
        //        //   loc.Id = ad.Id;
        //        if (SaveOrUpdate == "Save")
        //        {
        //            db.AdsLocations.Add(loc);
        //        }
        //        else if (SaveOrUpdate == "Update")
        //        {
        //            db.Entry(loc).State = EntityState.Modified;
        //        }
        //        db.SaveChanges();
        //        return Ok();
        //    }
        //}
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
        // PUT api/Company/5
        public async Task<IHttpActionResult> PutCompany(int id, Company company)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != company.Id)
            {
                return BadRequest();
            }

            db.Entry(company).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyExists(id))
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

        // POST api/Company
        [ResponseType(typeof(Company))]
        public async Task<IHttpActionResult> PostCompany(Company company)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Companies.Add(company);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = company.Id }, company);
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