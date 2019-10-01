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
using KKSOFDemoApp.Models;
using Newtonsoft.Json;

namespace KKSOFDemoApp.Controllers
{
    public class MessageController : ApiController
    {
        private KKSOFDBEntities db = new KKSOFDBEntities();



        [Route("api/citizenmessages")]
        public HttpResponseMessage GetCitizenMessages(Guid CitizenId)
        {
            try
            {
                var httpResponseMessage = new HttpResponseMessage();
                var CitizenMessages = (from p in db.Messages
                                        where p.CitizenId == CitizenId
                                        select new
                                        {
                                            Date = p.Createdtimestamp,
                                            Status = p.Status,
                                            Description = p.Description,
                                            MessageType = p.MessageType,
                                            Title = p.Title
                                        }).Take(3).ToList();
                httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject(CitizenMessages));
                httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                return httpResponseMessage;
            }
            catch
            {
                return null;
            }

        }


        [Route("api/allmessages")]
        public HttpResponseMessage GetAllMessages()
        {
            try
            {
                var httpResponseMessage = new HttpResponseMessage();
                var Messages = (from p in db.Messages 
                                       join c in db.Security_Citizens
                                       on p.CitizenId equals c.GUID
                                       join d in db.Security_User
                                       on p.recipient equals d.GUID into grpjoin_user
                                       from d in grpjoin_user.DefaultIfEmpty()
                                       orderby p.Createdtimestamp descending                                       
                                       select new
                                       {
                                           MessageDate = p.Createdtimestamp,
                                           Status = p.Status,
                                           Description = p.Description,
                                           Title = p.Title,
                                           CitizenGUID = c.GUID,
                                           Fullname = c.Firstname + " " + c.Lastname,
                                           PersonalId = c.PersonalIdNo,
                                           MessageType = p.MessageType,
                                           messageStatus = p.MessageStatus,
                                           Recipient = d.Firstname + " " + d.Lastname + " (" + d.UserId + ")"
                                       }).ToList();
                httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject(Messages));
                httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                return httpResponseMessage;
            }
            catch
            {
                return null;
            }

        }



        [HttpPost]
        [Route("api/FilterMessages")]
        public HttpResponseMessage SearchMessages(MessageData messageData)
        {
            try
            {
                var httpResponseMessage = new HttpResponseMessage();
                httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject(db.Get_FilteredMessages(messageData.Fromdate, messageData.Todate, messageData.type, messageData.messageStatus, messageData.citizenInfo, messageData.text,messageData.Responsible).ToList()));
                httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                return httpResponseMessage;
            }
            catch
            {
                return null;
            }
        }

        [HttpPost]
        [Route("api/FilterCitizenMessages")]
        public HttpResponseMessage SearchcitizenFilteredMessages(MessageData messageData)
        {
            try
            {
                var httpResponseMessage = new HttpResponseMessage();
                httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject(db.Get_FilteredCitizenMessages(messageData.Fromdate, messageData.Todate, messageData.type, messageData.messageStatus, messageData.citizenInfo, messageData.text, messageData.CitizenId).ToList()));
                httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                return httpResponseMessage;
            }
            catch
            {
                return null;
            }
        }

        // GET api/Message
        public IQueryable<Message> GetMessages()
        {
            return db.Messages;
        }

        // GET api/Message/5
        [ResponseType(typeof(Message))]
        public IHttpActionResult GetMessage(Guid id)
        {
            Message message = db.Messages.Find(id);
            if (message == null)
            {
                return NotFound();
            }

            return Ok(message);
        }

        // PUT api/Message/5
        public IHttpActionResult PutMessage(Guid id, Message message)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != message.GUID)
            {
                return BadRequest();
            }

            db.Entry(message).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MessageExists(id))
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

        // POST api/Message
        [ResponseType(typeof(Message))]
        public HttpResponseMessage PostMessage(Message message)
        {
            var httpResponseMessage = new HttpResponseMessage();

            DateTime updatedTimestamp = DateTime.Now;

            Message citizenMessage = new Message
            {
                GUID = Guid.NewGuid(),
                CitizenId = message.CitizenId,
                Createdtimestamp = updatedTimestamp,
                Title = null,
                Deleted = false,
                Description = message.Description,
                MessageType = message.MessageType,
                Status = message.Status,
                MessageStatus = message.MessageStatus,
                Log_from = updatedTimestamp,
                recipient = message.recipient,
                Log_UserId = message.Log_UserId
            };

            db.Messages.Add(citizenMessage);

            try
            {
                db.SaveChanges();
                httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject("true"));
                httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                return httpResponseMessage;
            }
            catch
            {
                httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject("false"));
                httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                return httpResponseMessage;
            }
        }

        // DELETE api/Message/5
        [ResponseType(typeof(Message))]
        public IHttpActionResult DeleteMessage(Guid id)
        {
            Message message = db.Messages.Find(id);
            if (message == null)
            {
                return NotFound();
            }

            db.Messages.Remove(message);
            db.SaveChanges();

            return Ok(message);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MessageExists(Guid id)
        {
            return db.Messages.Count(e => e.GUID == id) > 0;
        }
    }

    public class MessageData
    {
        public Nullable<System.DateTime> Fromdate { get; set; }
        public Nullable<System.DateTime> Todate { get; set; }        
        public string type { get; set; }
        public string messageStatus { get; set; }
        public string citizenInfo { get; set; }
        public string text { get; set; }
        public Nullable<System.Guid> CitizenId { get; set; }
        public Nullable<System.Guid> Responsible { get; set; }
    }
}