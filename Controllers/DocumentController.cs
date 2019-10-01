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
    public class DocumentController : ApiController
    {
        private KKSOFDBEntities db = new KKSOFDBEntities();

        [Route("api/citizendocuments")]
        public HttpResponseMessage GetCitizenDocuments(Guid CitizenId)
        {
            try
            {
                var httpResponseMessage = new HttpResponseMessage();
                var CitizenDocuments = (from p in db.Documents
                                        join c in db.Security_User
                                         on p.Log_UserId equals c.GUID
                                        where p.CitizenId == CitizenId
                                        orderby p.Log_from descending
                                        select new
                                        {
                                            Guid = p.GUID,
                                            Date = p.Createdtimestamp,
                                            Description = p.Description,
                                            DocType = p.DocumentType,
                                            DocName = p.Name,
                                            Username = c.Firstname + " " + c.Lastname + " (" + c.UserId + ")"
                                        }).ToList();
                httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject(CitizenDocuments));
                httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                return httpResponseMessage;
            }
            catch
            {
                return null;
            }

        }



        // GET api/Document
        public HttpResponseMessage GetDocuments()
        {

            try
            {

                var documents = (from p in db.Documents
                                 join c in db.Security_User
                                 on p.Log_UserId equals c.GUID
                                 join d in db.Security_Citizens
                                 on p.CitizenId equals d.GUID                                 
                                 select new
                                 {
                                     CreatedDate = p.Createdtimestamp,
                                     NoteType = p.DocumentType, 
                                     Noteheadline = p.Name,
                                     NoteText = p.Description,
                                     Username = c.Firstname + " " + c.Lastname + " (" + c.UserId + ")",
                                     CitizenName = d.Firstname + " " + d.Lastname,
                                     CitizenId = d.PersonalIdNo
                                 }).ToList();


                var httpResponseMessage = new HttpResponseMessage();
                httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject(documents));
                httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                return httpResponseMessage;
            }
            catch
            {
                return null;
            }
        }


        [HttpPost]
        [Route("api/FilterDocuments")]
        public HttpResponseMessage SearchDocuments(DocumentData documentData)
        {
            try
            {
                var httpResponseMessage = new HttpResponseMessage();
                httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject(db.Get_FilteredNotes(documentData.Fromdate, documentData.Todate, documentData.DocType, documentData.Name, documentData.UserId, documentData.NoteText).ToList()));
                httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                return httpResponseMessage;
            }
            catch
            {
                return null;
            }
        }

        [HttpPost]
        [Route("api/FilterCitizenDocuments")]
        public HttpResponseMessage SearchCitizenDocuments(DocumentData documentData)
        {
            try
            {
                var httpResponseMessage = new HttpResponseMessage();
                httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject(db.Get_FilteredCitizenNotes(documentData.Fromdate, documentData.Todate, documentData.DocType, documentData.Name, documentData.UserId, documentData.NoteText, documentData.CitizenId).ToList()));
                httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                return httpResponseMessage;
            }
            catch
            {
                return null;
            }
        }

        // GET api/Document/5
        [ResponseType(typeof(Document))]
        public HttpResponseMessage GetDocument(Guid id)
        {
            try
            {
                DateTime updatedTimestamp = DateTime.Now;
                var citizendocument = (from p in db.Documents
                                       join c in db.Security_User
                                       on p.Log_UserId equals c.GUID
                                       where p.GUID == id
                                       select new
                                       {
                                           CreatedDate = p.Createdtimestamp,
                                           NoteType = p.DocumentType,
                                           Noteheadline = p.Name,
                                           NoteText = p.Description,
                                           Username = c.UserId,
                                           FullName = c.Firstname + " " + c.Lastname,
                                           Profession = c.Profession
                                       }).FirstOrDefault();


                var httpResponseMessage = new HttpResponseMessage();
                httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject(citizendocument));
                httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                return httpResponseMessage;
            }
            catch
            {
                return null;
            }
        }

        // PUT api/Document/5
        public HttpResponseMessage PutDocument(Guid id, Document document)
        {
            try
            {
                DateTime updatedTimestamp = DateTime.Now;
                Document citizendocument = (from p in db.Documents
                                      where p.GUID == id
                                     select p).FirstOrDefault();


                citizendocument.Createdtimestamp = document.Createdtimestamp;
                citizendocument.Description = document.Description;
                citizendocument.Log_from = updatedTimestamp;
                citizendocument.Name = document.Name;
                citizendocument.Log_UserId = document.Log_UserId;
                citizendocument.DocumentType = document.DocumentType;
                db.SaveChanges();

                var httpResponseMessage = new HttpResponseMessage();
                httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject("true"));
                httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                return httpResponseMessage;
            }
            catch
            {
                return null;
            }
        }

        // POST api/Document
        [ResponseType(typeof(Document))]
        public HttpResponseMessage PostDocument(Document document)
        {
            var httpResponseMessage = new HttpResponseMessage();

            DateTime updatedTimestamp = DateTime.Now;

            Document NoteDocument = new Document
            {
                GUID = Guid.NewGuid(),
                CitizenId = document.CitizenId,
                Createdtimestamp = document.Createdtimestamp,
                Deleted = false,
                Description = document.Description,
                DocumentType = document.DocumentType,
                DocURL = null,
                FollowupID = null,
                Log_from = updatedTimestamp,
                Name = document.Name,
                Log_UserId = document.Log_UserId
            };

            db.Documents.Add(NoteDocument);

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

        // DELETE api/Document/5
        [ResponseType(typeof(Document))]
        public IHttpActionResult DeleteDocument(Guid id)
        {
            Document document = db.Documents.Find(id);
            if (document == null)
            {
                return NotFound();
            }

            db.Documents.Remove(document);
            db.SaveChanges();

            return Ok(document);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DocumentExists(Guid id)
        {
            return db.Documents.Count(e => e.GUID == id) > 0;
        }
    }

    public class DocumentData
    {
        public Nullable<System.DateTime> Fromdate { get; set; }
        public Nullable<System.DateTime> Todate { get; set; }        
        public Nullable<byte> DocType { get; set; }
        public string Name { get; set; }
        public Nullable<System.Guid> UserId { get; set; }
        public string NoteText { get; set; }
        public Nullable<System.Guid> CitizenId { get; set; }
    }
}