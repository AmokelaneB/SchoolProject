using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Online_Store_API.Models;

namespace Online_Store_API.Controllers.Scaffolds
{
    public class EstablishmentsController : ApiController
    {
        private GradStockUpEntities db = new GradStockUpEntities();

        // GET: api/Establishments
        public IQueryable<Establishment> GetEstablishments()
        {
            return db.Establishments;
        }

        // GET: api/Establishments/5
        [ResponseType(typeof(Establishment))]
        public IHttpActionResult GetEstablishment(int id)
        {
            Establishment establishment = db.Establishments.Find(id);
            if (establishment == null)
            {
                return NotFound();
            }

            return Ok(establishment);
        }

        // PUT: api/Establishments/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutEstablishment(int id, Establishment establishment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != establishment.EstablishmentID)
            {
                return BadRequest();
            }

            db.Entry(establishment).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EstablishmentExists(id))
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

        // POST: api/Establishments
        [ResponseType(typeof(Establishment))]
        public IHttpActionResult PostEstablishment(Establishment establishment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Establishments.Add(establishment);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = establishment.EstablishmentID }, establishment);
        }

        // DELETE: api/Establishments/5
        [ResponseType(typeof(Establishment))]
        public IHttpActionResult DeleteEstablishment(int id)
        {
            Establishment establishment = db.Establishments.Find(id);
            if (establishment == null)
            {
                return NotFound();
            }

            db.Establishments.Remove(establishment);
            db.SaveChanges();

            return Ok(establishment);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EstablishmentExists(int id)
        {
            return db.Establishments.Count(e => e.EstablishmentID == id) > 0;
        }
    }
}