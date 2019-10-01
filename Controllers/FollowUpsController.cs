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
    public class FollowUpsController : ApiController
    {
        private KKSOFDBEntities db = new KKSOFDBEntities();

        // GET: api/FollowUps
        public IQueryable<Citizen_FollowUps> GetCitizen_FollowUps()
        {
            return db.Citizen_FollowUps;
        }




        [HttpGet]
        [Route("api/getfollowup/{citizenid}")]
        public HttpResponseMessage GetCitizen_FollowUps(Guid citizenid)
        {
            try
            {

                Citizen_FollowUps followUp = (from p in db.Citizen_FollowUps
                                      where p.CitizenId == citizenid
                                      select p).FirstOrDefault();

                if (followUp != null)
                {
                    Guid?[] items = (from c in db.Citizen_FollowUpAttachments
                                           where c.FollowupId == followUp.GUID
                                           select c.Objectid
                                          ).ToArray();


                    FollowupData final = new FollowupData
                    {
                        GUID = followUp.GUID,
                        CitizenId = followUp.CitizenId,
                        Createdtimestamp = followUp.Createdtimestamp,
                        NextFollowupdate = followUp.NextFollowupdate
                    };
                    List<FollowupAtatchedNotes> attachments = new List<FollowupAtatchedNotes>();

                    if(items.Length > 0)
                    {
                        foreach(Guid? attachmentGUID in items)
                        {
                            if (attachmentGUID == null)
                                final.IsDocumentA = true;
                            else
                            {
                                var doc = (from d in db.Documents
                                           where d.GUID == attachmentGUID
                                           select d).FirstOrDefault();

                                FollowupAtatchedNotes note = new FollowupAtatchedNotes
                                {
                                    GUID = doc.GUID,
                                    Createdtimestamp = doc.Createdtimestamp,
                                    Description = doc.Description,
                                    DocumentType = doc.DocumentType
                                };

                                attachments.Add(note);
                            }
                        }
                    }

                    final.noteAttachments = attachments.ToArray();

                    var httpResponseMessage = new HttpResponseMessage();
                    httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject(final));
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

        [HttpPut]
        [Route("api/updatefollowup/{followup_id}")]
        public HttpResponseMessage PutCitizen_FollowUps(Guid followup_id, FollowupData followUp)
        {
            try
            {
                DateTime updatedTimestamp = DateTime.Now;

                Citizen_FollowUps citizenFollowUp = (from p in db.Citizen_FollowUps
                                                     where p.GUID == followup_id
                                                     select p).FirstOrDefault();


                citizenFollowUp.NextFollowupdate = followUp.NextFollowupdate;
                citizenFollowUp.Log_from = updatedTimestamp;
                citizenFollowUp.Log_UserId = followUp.Log_UserId;

                List<Citizen_FollowUpAttachments> maps = (from p in db.Citizen_FollowUpAttachments
                                                          where p.FollowupId == followup_id
                                                          select p).ToList();

                if (maps.Count > 0)
                {
                    foreach (Citizen_FollowUpAttachments item in maps)
                    {
                        db.Citizen_FollowUpAttachments.Remove(item);
                    }
                }

                if (followUp.attachments.Length > 0)
                {
                    foreach (Attachments item in followUp.attachments)
                    {
                        Citizen_FollowUpAttachments followupAttachments = new Citizen_FollowUpAttachments
                        {
                            GUID = Guid.NewGuid(),
                            Createdtimestamp = updatedTimestamp,
                            Log_UserId = followUp.Log_UserId,
                            CitizenId = followUp.CitizenId,
                            FollowupId = followup_id,
                            DocType = item.DocType,
                            Docname = item.Docname,
                            Objectid = item.Objectid
                        };
                        db.Citizen_FollowUpAttachments.Add(followupAttachments);
                    }
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

        [HttpPost]
        [Route("api/savefollowup")]
        public HttpResponseMessage PostCitizen_FollowUps(FollowupData followUp)
        {
            var httpResponseMessage = new HttpResponseMessage();

            DateTime updatedTimestamp = DateTime.Now;
            Guid followupID = Guid.NewGuid();

            Citizen_FollowUps citizenFollowup = new Citizen_FollowUps
            {
                GUID = followupID,
                Createdtimestamp = updatedTimestamp,
                Log_from = updatedTimestamp,
                Log_UserId = followUp.Log_UserId,
                CitizenId = followUp.CitizenId,
                NextFollowupdate = followUp.NextFollowupdate,
                Description = null,
                Consent = null,
                CaseWorkerdescription = null,
                StatusNoteID = followUp.StatusNoteID,
                Result = null,
                Deleted = false
            };

            db.Citizen_FollowUps.Add(citizenFollowup);

            if (followUp.attachments.Length > 0)
            {
                foreach (Attachments item in followUp.attachments)
                {
                    Citizen_FollowUpAttachments followupAttachments = new Citizen_FollowUpAttachments
                    {
                        GUID = Guid.NewGuid(),
                        Createdtimestamp = updatedTimestamp,
                        Log_UserId = followUp.Log_UserId,
                        CitizenId = followUp.CitizenId,
                        FollowupId = followupID,
                        DocType = item.DocType,
                        Docname = item.Docname,
                        Objectid = item.Objectid
                    };

                    db.Citizen_FollowUpAttachments.Add(followupAttachments);
                }
            }


            var deadline = (from p in db.Deadlines
                            where p.CitizenId ==  followUp.CitizenId && p.DeadlineColorEnum == 3
                            select p).FirstOrDefault();

            if (deadline != null)
            {
                deadline.Completedby = followUp.Log_UserId;
                deadline.DeadlineColorEnum = 2;
                deadline.DeadlineDate = followUp.NextFollowupdate;
                deadline.CompletedDate = updatedTimestamp;
                deadline.Log_from = updatedTimestamp;
                deadline.Log_UserId = followUp.Log_UserId;
                deadline.Title = "Opfølgning på indsatsmål";
            }

            try
            {
                db.SaveChanges();
                httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject(followupID));
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






         
        [ResponseType(typeof(Citizen_FollowUps))]
        public IHttpActionResult DeleteCitizen_FollowUps(Guid id)
        {
            Citizen_FollowUps citizen_FollowUps = db.Citizen_FollowUps.Find(id);
            if (citizen_FollowUps == null)
            {
                return NotFound();
            }

            db.Citizen_FollowUps.Remove(citizen_FollowUps);
            db.SaveChanges();

            return Ok(citizen_FollowUps);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool Citizen_FollowUpsExists(Guid id)
        {
            return db.Citizen_FollowUps.Count(e => e.GUID == id) > 0;
        }
    }


    public class FollowupData
    {
        public System.Guid GUID { get; set; }
        public System.DateTime Createdtimestamp { get; set; }
        public System.DateTime Log_from { get; set; }
        public System.Guid Log_UserId { get; set; }
        public System.Guid CitizenId { get; set; }
        public Nullable<System.DateTime> NextFollowupdate { get; set; }
        public string Description { get; set; }
        public string Consent { get; set; }
        public string CaseWorkerdescription { get; set; }
        public System.Guid StatusNoteID { get; set; }
        public Nullable<byte> Result { get; set; }
        public bool Deleted { get; set; }
        public bool IsDocumentA { get; set; }
        public Attachments[] attachments { get; set; }
        public FollowupAtatchedNotes[] noteAttachments { get; set; }
    }

    public class Attachments
    {
        public string DocType { get; set; }
        public string Docname { get; set; }
        public Nullable<System.Guid> Objectid { get; set; }
    }



    public class FollowupAtatchedNotes
    {
        public System.Guid GUID { get; set; }
        public System.DateTime Createdtimestamp { get; set; }
        public byte DocumentType { get; set; }
        public string Description { get; set; }
    }
}