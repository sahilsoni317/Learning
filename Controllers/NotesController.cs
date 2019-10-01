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
    public class NotesController : ApiController
    {
        private KKSOFDBEntities db = new KKSOFDBEntities();

        // GET api/Notes
        public HttpResponseMessage GetCitizen_Notes()
        {
            try
            {
                var httpResponseMessage = new HttpResponseMessage();
                var StatusNotes = (from p in db.Citizen_Notes
                                 join c in db.Security_Citizens
                                 on p.CitizenId equals c.GUID                                 
                                 orderby p.Createdtimestamp
                                 select new
                                 {
                                     NoteCreateddate = p.Createdtimestamp,
                                     NoteHeadline = p.Headline,                                    
                                     CitizenfirstName = c.Firstname,
                                     CitizenLastName = c.Lastname,
                                     PersonalId = c.PersonalIdNo
                                 }).ToList();
                httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject(StatusNotes));
                httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                return httpResponseMessage;
            }
            catch
            {
                return null;
            }
        }

        // GET api/Notes/5
        [ResponseType(typeof(Citizen_Notes))]
        public IHttpActionResult GetCitizen_Notes(Guid id)
        {
            Citizen_Notes citizen_notes = db.Citizen_Notes.Find(id);
            if (citizen_notes == null)
            {
                return NotFound();
            }

            return Ok(citizen_notes);
        }


        [HttpGet]
        [Route("api/getstatusnote/{citizenid}")]
        public HttpResponseMessage GetCitizen_Note(Guid citizenid)
        {
            try
            {

                Citizen_Notes note = (from p in db.Citizen_Notes                                      
                                      where p.CitizenId == citizenid
                                      select p).FirstOrDefault();

                if(note != null)
                {
                    Guid[] items = (from c in db.CitizenstatusNoteMappings
                                 where c.StatusNoteId == note.GUID
                                 select c.UserId
                                 ).ToArray();

                    StatusNote statusNote = new StatusNote
                    {
                        GUID = note.GUID,
                        Createdtimestamp = note.Createdtimestamp.ToLocalTime(),
                        Description = note.Description,
                        Score = note.Score,
                        Log_UserId = note.Log_UserId,
                        CitizenId = note.CitizenId,
                        UserIds = items
                    };

                    var httpResponseMessage = new HttpResponseMessage();
                    httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject(statusNote));
                    httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    return httpResponseMessage;
                  
                }
                else
                {
                    var httpResponseMessage = new HttpResponseMessage();
                    httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject("false"));
                    httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    return httpResponseMessage;
                  
                }

            
            }
            catch
            {
                return null;
            }
        }

        // PUT api/Notes/5
        [HttpPut]
        [Route("api/updatestatusnote/{status_note_id}")]
        public HttpResponseMessage PutCitizen_Notes(Guid status_note_id, StatusNote statusNote)
        {
            try
            {
                DateTime updatedTimestamp = DateTime.Now;

                Citizen_Notes note = (from p in db.Citizen_Notes
                                      where p.GUID == status_note_id
                                            select p).FirstOrDefault();


                note.Createdtimestamp = statusNote.Createdtimestamp;
                note.Description = statusNote.Description;
                note.Log_from = updatedTimestamp;
                note.Score = statusNote.Score;
                note.Log_UserId = statusNote.Log_UserId;

               List<CitizenstatusNoteMapping> maps = (from p in db.CitizenstatusNoteMappings
                                                      where p.StatusNoteId == status_note_id
                                                 select p).ToList();

               if (maps.Count > 0)
               {
                   foreach (CitizenstatusNoteMapping item in maps)
                   {
                       db.CitizenstatusNoteMappings.Remove(item);
                   }
               }

               if (statusNote.UserIds.Length > 0)
               {
                   foreach (Guid item in statusNote.UserIds)
                   {
                       CitizenstatusNoteMapping notemapping = new CitizenstatusNoteMapping
                       {
                           GUID = Guid.NewGuid(),
                           StatusNoteId = status_note_id,
                           UserId = item
                       };
                       db.CitizenstatusNoteMappings.Add(notemapping);
                   }
               }

               //Message citizenMessage = new Message
               //{
               //    GUID = Guid.NewGuid(),
               //    CitizenId = note.CitizenId,
               //    Createdtimestamp = updatedTimestamp,
               //    Title = null,
               //    Deleted = false,
               //    Description = "Status Note Saved",
               //    MessageType = "StatusNotat",
               //    Status = 2,
               //    MessageStatus = "Ubehandlet",
               //    Log_from = updatedTimestamp,
               //    recipient = null,
               //    Log_UserId = note.Log_UserId
               //};

               //db.Messages.Add(citizenMessage);


               Document citizendocument = (from p in db.Documents
                                           where p.CitizenId == note.CitizenId && p.DocumentType == 2
                                           select p).FirstOrDefault();

               if (citizendocument != null)
               {
                   citizendocument.Createdtimestamp = note.Createdtimestamp;
                   citizendocument.Description = note.Description;
                   citizendocument.Log_from = updatedTimestamp;
                   citizendocument.Log_UserId = note.Log_UserId;
               }

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

        // POST api/Notes
        [HttpPost]
        [Route("api/savestatusnote")]
        public HttpResponseMessage PostCitizen_Notes(StatusNote statusNote)
        {

            var httpResponseMessage = new HttpResponseMessage();

            DateTime updatedTimestamp = DateTime.Now;
            Guid StatusNoteId = Guid.NewGuid();

            Citizen_Notes Note = new Citizen_Notes
            {
                GUID = StatusNoteId,
                Createdtimestamp = statusNote.Createdtimestamp.ToUniversalTime(),
                Log_from = updatedTimestamp,
                Log_UserId = statusNote.Log_UserId,
                Score = statusNote.Score,
                Description = statusNote.Description,
                CitizenId = statusNote.CitizenId,
                Participants = null,
                SaveDraft = false,
                Headline = null,
                Notetype = 1,
                Deleted = false,
                FollowupId = null
            };

            db.Citizen_Notes.Add(Note);

            if (statusNote.UserIds.Length > 0)
            {
                foreach (Guid item in statusNote.UserIds)
                {
                    CitizenstatusNoteMapping notemapping = new CitizenstatusNoteMapping
                    {
                        GUID = Guid.NewGuid(),
                        StatusNoteId = StatusNoteId,
                        UserId = item
                    };
                    db.CitizenstatusNoteMappings.Add(notemapping);
                }
            }

            Message citizenMessage = new Message
            {
                GUID = Guid.NewGuid(),
                CitizenId = Note.CitizenId,
                Createdtimestamp = updatedTimestamp,
                Title = null,
                Deleted = false,
                Description = "Statusnotat oprettet",
                MessageType = "Statusnotat",
                Status = 2,
                MessageStatus = "Ubehandlet",
                Log_from = updatedTimestamp,
                recipient = null,
                Log_UserId = Note.Log_UserId
            };

            db.Messages.Add(citizenMessage);



            Document NoteDocument = new Document
            {
                GUID = Guid.NewGuid(),
                CitizenId = Note.CitizenId,
                Createdtimestamp = updatedTimestamp,
                Deleted = false,
                Description = Note.Description,
                DocumentType = 2,
                DocURL = null,
                FollowupID = null,
                Log_from = updatedTimestamp,
                Name = "",
                Log_UserId = Note.Log_UserId
            };

            db.Documents.Add(NoteDocument);

            try
            {
                db.SaveChanges();
                httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject(StatusNoteId));
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

        // DELETE api/Notes/5
        [ResponseType(typeof(Citizen_Notes))]
        public IHttpActionResult DeleteCitizen_Notes(Guid id)
        {
            Citizen_Notes citizen_notes = db.Citizen_Notes.Find(id);
            if (citizen_notes == null)
            {
                return NotFound();
            }

            db.Citizen_Notes.Remove(citizen_notes);
            db.SaveChanges();

            return Ok(citizen_notes);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool Citizen_NotesExists(Guid id)
        {
            return db.Citizen_Notes.Count(e => e.GUID == id) > 0;
        }
    }

    public class StatusNote
    {
        public System.Guid GUID { get; set; }
        public System.DateTime Createdtimestamp { get; set; }
        public System.DateTime Log_from { get; set; }
        public System.Guid Log_UserId { get; set; }
        public byte Score { get; set; }
        public string Description { get; set; }
        public System.Guid CitizenId { get; set; }
        public string Participants { get; set; }
        public bool SaveDraft { get; set; }
        public string Headline { get; set; }
        public Nullable<byte> Notetype { get; set; }
        public bool Deleted { get; set; }
        public Nullable<System.Guid> FollowupId { get; set; }
        public System.Guid[] UserIds { get; set; }

    }
}