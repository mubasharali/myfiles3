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

namespace Inspinia_MVC5_SeedProject.Controllers
{
    public class VehicleController : ApiController
    {
        private Entities db = new Entities();

        
       
        [HttpPost]
        public async Task<IHttpActionResult> GetCarTree()
        {
            var mobiles = (from mobile in db.CarBrands.ToList()
                           orderby mobile.Id
                           select new
                           {
                               companyName = mobile.Id,
                               models = from model in mobile.CarModels.ToList()
                                        select new
                                        {
                                            model = model.model
                                        }
                           }).AsEnumerable();
            return Ok(mobiles);
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetCarBrands()
        {
            var brands = (db.CarBrands.Select(x => x.Id)).AsEnumerable();
            return Ok(brands);
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetCarModels(string brand)
        {
            var models = await db.CarModels.Where(x => x.brand == brand).Select(x => x.model).ToListAsync();
            return Ok(models);
        }
        // GET api/Vehicle/5
        [ResponseType(typeof(Ad))]
        public async Task<IHttpActionResult> GetAd(int id)
        {
            Ad ad = await db.Ads.FindAsync(id);
            if (ad == null)
            {
                return NotFound();
            }

            return Ok(ad);
        }

        // PUT api/Vehicle/5
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

        // POST api/Vehicle
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

        // DELETE api/Vehicle/5
        [ResponseType(typeof(Ad))]
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