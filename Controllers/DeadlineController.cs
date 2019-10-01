using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Data.Objects;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using KKSOFDemoApp.Models;
using Newtonsoft.Json;

namespace KKSOFDemoApp.Controllers
{
    public class DeadlineController : ApiController
    {
        private KKSOFDBEntities db = new KKSOFDBEntities();

        // GET api/Deadline
        public HttpResponseMessage GetDeadlines()
        {
            try
            {
                var httpResponseMessage = new HttpResponseMessage();
                var deadlines = (from p in db.Deadlines
                                 join c in db.Security_Citizens
                                 on p.CitizenId equals c.GUID
                                 join d in db.Security_User
                                       on p.Log_UserId equals d.GUID
                                 where (p.DeadlineColorEnum == 2 || p.DeadlineColorEnum ==3)
                                 orderby p.DeadlineDate
                                 select new
                                 {
                                     DeadlineDate =  p.DeadlineDate,
                                     Deadlinecolor = p.DeadlineColorEnum,
                                     DeadlineTitle = p.Title,
                                     Deadlinetype = p.DeadlineOwnertype,
                                     UserName = d.Firstname + " " + d.Lastname + " (" + d.UserId + ")",
                                     CitizenfirstName = c.Firstname,
                                     CitizenLastName = c.Lastname,
                                     PersonalId = c.PersonalIdNo,
                                     CitizenId = c.GUID
                                 }).Take(4).ToList();
                httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject(deadlines));
                httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                return httpResponseMessage;
            }
            catch
            {
                return null;
            }
          
        }


        [Route("api/reddeadlines")]
        public HttpResponseMessage GetRedDeadlines()
        {
            try
            {
                var httpResponseMessage = new HttpResponseMessage();
                var redDeadlines = (from p in db.Deadlines
                                 join c in db.Security_Citizens
                                 on p.CitizenId equals c.GUID
                                    join d in db.Security_User
                                          on p.Log_UserId equals d.GUID
                                 where (p.DeadlineColorEnum == 1)
                                 orderby p.DeadlineDate
                                 select new
                                 {
                                     DeadlineDate = p.DeadlineDate,
                                     Deadlinecolor = p.DeadlineColorEnum,
                                     DeadlineTitle = p.Title,
                                     Deadlinetype = p.DeadlineOwnertype,
                                     UserName = d.Firstname + " " + d.Lastname + " (" + d.UserId + ")",
                                     CitizenfirstName = c.Firstname,
                                     CitizenLastName = c.Lastname,
                                     PersonalId = c.PersonalIdNo
                                 }).ToList();
                httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject(redDeadlines));
                httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                return httpResponseMessage;
            }
            catch
            {
                return null;
            }

        }

        [Route("api/alldeadlines")]
        public HttpResponseMessage GetAllDeadlines()
        {
            try
            {
                var httpResponseMessage = new HttpResponseMessage();
                var alldeadlines = (from p in db.Deadlines
                                    join c in db.Security_Citizens
                                    on p.CitizenId equals c.GUID                                    
                                    orderby p.DeadlineDate
                                    select new
                                    {
                                        DeadlineDate = p.DeadlineDate,
                                        Deadlinecolor = p.DeadlineColorEnum,
                                        DeadlineTitle = p.Title,
                                        CitizenfirstName = c.Firstname,
                                        CitizenLastName = c.Lastname,
                                        PersonalId = c.PersonalIdNo
                                    }).ToList();
                httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject(alldeadlines));
                httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                return httpResponseMessage;
            }
            catch
            {
                return null;
            }

        }

        [HttpPost]
        [Route("api/FilterDeadlines")]
        public HttpResponseMessage Searchdeadlines(DeadlineData deadlineData)
        {
            try
            {
                var httpResponseMessage = new HttpResponseMessage();
                httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject(db.Get_FilteredDeadlines(deadlineData.citizenInfo,deadlineData.isRedDeadline).ToList()));
                httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                return httpResponseMessage;
            }
            catch
            {
                return null;
            }
        }




        [Route("api/citizendeadline")]
        public HttpResponseMessage GetCitizenDeadlines(Guid CitizenId)
        {
            try
            {
                var httpResponseMessage = new HttpResponseMessage();
                var CitizenDeadlines = (from p in db.Deadlines
                                         join c in db.Security_Citizens
                                         on p.CitizenId equals c.GUID
                                        join d in db.Security_User
                                        on p.Log_UserId equals d.GUID
                                        where p.CitizenId == CitizenId                                    
                                    select new
                                    {
                                        DeadlineDate = p.DeadlineDate,
                                        Deadlinecolor = p.DeadlineColorEnum,
                                        DeadlineTitle = p.Title, 
                                        Deadlinetype = p.DeadlineOwnertype,
                                        UserName = d.Firstname + " " + d.Lastname + " (" + d.UserId + ")",
                                        CitizenfirstName = c.Firstname,
                                        CitizenLastName = c.Lastname,
                                        PersonalId = c.PersonalIdNo
                                         
                                    }).ToList();
                httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject(CitizenDeadlines));
                httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                return httpResponseMessage;
            }
            catch
            {
                return null;
            }

        }
             
        private string GetDeadlineType(int type)
        {
            if (type == 1)
            {
                return "Opfølgningsfrist";
            }
            else if (type == 2)
            {
                return "Sagsbehandlingsfrist";
            }
            else if (type == 3)
            {
                return "Kvitteringsfrist";
            }
            else if (type == 4)
            {
                return "Parthøringsfrist";
            }
            else if (type == 5)
            {
                return "Klagefrist";
            }
            else
                return null;
        }


        // GET api/Deadline/5
        [ResponseType(typeof(Deadline))]
        public IHttpActionResult GetDeadline(Guid id)
        {
            Deadline deadline = db.Deadlines.Find(id);
            if (deadline == null)
            {
                return NotFound();
            }

            return Ok(deadline);
        }

        // PUT api/Deadline/5
        public IHttpActionResult PutDeadline(Guid id, Deadline deadline)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != deadline.GUID)
            {
                return BadRequest();
            }

            db.Entry(deadline).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeadlineExists(id))
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

        // POST api/Deadline
        [ResponseType(typeof(Deadline))]
        public IHttpActionResult PostDeadline(Deadline deadline)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Deadlines.Add(deadline);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (DeadlineExists(deadline.GUID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = deadline.GUID }, deadline);
        }

        // DELETE api/Deadline/5
        [ResponseType(typeof(Deadline))]
        public IHttpActionResult DeleteDeadline(Guid id)
        {
            Deadline deadline = db.Deadlines.Find(id);
            if (deadline == null)
            {
                return NotFound();
            }

            db.Deadlines.Remove(deadline);
            db.SaveChanges();

            return Ok(deadline);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DeadlineExists(Guid id)
        {
            return db.Deadlines.Count(e => e.GUID == id) > 0;
        }
    }

    public class DeadlineData
    {
        public string citizenInfo { get; set; }
        public Nullable<bool> isRedDeadline { get; set; }
        public Nullable<System.Guid> CitizenId { get; set; }
    }
}