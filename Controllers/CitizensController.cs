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
using System.ComponentModel.DataAnnotations;
using KKSOFDemoApp.Models;
using Newtonsoft.Json;


namespace KKSOFDemoApp.Controllers
{
    public class CitizensController : ApiController
    {
        private KKSOFDBEntities db = new KKSOFDBEntities();

        // GET api/Citizens
        public IQueryable<Security_Citizens> GetSecurity_Citizens()
        {
            return db.Security_Citizens;
        }

        // GET api/Citizens/5
        [ResponseType(typeof(Security_Citizens))]
        public HttpResponseMessage GetSecurity_Citizens(Guid id)
        {

            try
            {
                var httpResponseMessage = new HttpResponseMessage();
                var Citizen = from p in db.Security_Citizens
                           where p.GUID == id
                           select new
                           {
                               Firstname = p.Firstname,
                               Lastname = p.Lastname,
                               Profession = p.Profession,
                               Address = p.Address,
                               City = p.City,
                               PersonalId = p.PersonalIdNo

                           };
                if (Citizen != null)
                {

                    httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject(Citizen));
                    httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    return httpResponseMessage;
                }
                else
                {
                    httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject("Not Found"));
                    httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    return httpResponseMessage;
                }
            }
            catch
            {
                return null;
            }                       
        }

        // PUT api/Citizens/5
        public IHttpActionResult PutSecurity_Citizens(Guid id, Security_Citizens security_citizens)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != security_citizens.GUID)
            {
                return BadRequest();
            }

            db.Entry(security_citizens).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Security_CitizensExists(id))
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

        // POST api/Citizens
        [ResponseType(typeof(Security_Citizens))]
        public IHttpActionResult PostSecurity_Citizens(Security_Citizens security_citizens)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Security_Citizens.Add(security_citizens);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (Security_CitizensExists(security_citizens.GUID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = security_citizens.GUID }, security_citizens);
        }

        // DELETE api/Citizens/5
        [ResponseType(typeof(Security_Citizens))]
        public IHttpActionResult DeleteSecurity_Citizens(Guid id)
        {
            Security_Citizens security_citizens = db.Security_Citizens.Find(id);
            if (security_citizens == null)
            {
                return NotFound();
            }

            db.Security_Citizens.Remove(security_citizens);
            db.SaveChanges();

            return Ok(security_citizens);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool Security_CitizensExists(Guid id)
        {
            return db.Security_Citizens.Count(e => e.GUID == id) > 0;
        }
    }
}