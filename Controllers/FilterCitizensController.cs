using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Web.Http;
using System.ComponentModel.DataAnnotations;
using KKSOFDemoApp.Models;
using Newtonsoft.Json;


namespace KKSOFDemoApp.Controllers
{
    [RoutePrefix("api/citizen")]
    public class FilterCitizensController : ApiController
    {
        private KKSOFDBEntities entities = new KKSOFDBEntities();

        [HttpGet]
        [Route("search/{criteria}")]
         public HttpResponseMessage SearchCitizens(string criteria)
        {
            try
            {
                var httpResponseMessage = new HttpResponseMessage();
                httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject(entities.GET_CITIZENS(criteria).ToList()));
                httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                return httpResponseMessage;
            }
            catch
            {
                return null;
            }
        }

        [HttpGet]
        [Route("getcitizenbyguid/{id}")]
        public HttpResponseMessage GetCitizenbyGUID(Guid id)
        {
            try
            {
                var httpResponseMessage = new HttpResponseMessage();
                httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject(entities.Get_CitizenbyGUID(id).ToList()));
                httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                return httpResponseMessage;
            }
            catch
            {
                return null;
            }
        }


        [HttpGet]
        [Route("backgroundinformation")]
        public HttpResponseMessage GetBackgroundInformation(Guid CitizenId)
        {
            try
            {
                var httpResponseMessage = new HttpResponseMessage();
                Citizen_BackgroundInfo bi = (from p in entities.Citizen_BackgroundInformation
                         where p.CitizenId == CitizenId
                         select new Citizen_BackgroundInfo()
                         {
                             GUID = p.GUID,
                             CitizenId = p.CitizenId,
                             HealthInformation = p.HealthInformation,
                             HealthInformation_Lastupdate = p.HealthInformation_Lastupdate,
                             LifeHistory = p.LifeHistory,
                             Lifehistory_Lastupdate = p.Lifehistory_Lastupdate,
                             MedicalHistory = p.MedicalHistory,
                             MedicalHistory_Lastupdate = p.MedicalHistory_Lastupdate,
                             SchoolInformation = p.SchoolInformation,
                             SchoolInformation_Lastupdate = p.SchoolInformation_Lastupdate,
                             SocialInformation = p.SocialInformation,
                             SocialInformation_Lastupdate = p.SocialInformation_Lastupdate
                         }).FirstOrDefault();

                bi.HealthInformation_Lastupdate = bi.HealthInformation_Lastupdate.Value.ToLocalTime();
                bi.Lifehistory_Lastupdate = bi.Lifehistory_Lastupdate.Value.ToLocalTime();
                bi.MedicalHistory_Lastupdate = bi.MedicalHistory_Lastupdate.Value.ToLocalTime();
                bi.SchoolInformation_Lastupdate = bi.SchoolInformation_Lastupdate.Value.ToLocalTime();
                bi.SocialInformation_Lastupdate = bi.SocialInformation_Lastupdate.Value.ToLocalTime();

                httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject(bi));
                httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                return httpResponseMessage;
            }
            catch
            {
                return null;
            }
        }

        [HttpGet]
        [Route("updatebackgroundinformation")]
        public HttpResponseMessage UpdateBackgroundInformation(Guid CitizenId,String Key,String Value)
        {
            try
            {
                DateTime updatedTimestamp = DateTime.Now.ToUniversalTime(); 
                Citizen_BackgroundInformation citizenbI = (from p in entities.Citizen_BackgroundInformation
                                                           where p.CitizenId == CitizenId
                                                          select p).FirstOrDefault();

                switch (Key)
                {
                    case "Sundhedsoplysninger-content":
                        {
                            if (citizenbI.HealthInformation == Value) { return null; }
                            citizenbI.HealthInformation = Value;
                            citizenbI.HealthInformation_Lastupdate = updatedTimestamp;
                            entities.SaveChanges();
                            break;
                        }
                    case "livshistorie-content":
                        {
                            if (citizenbI.LifeHistory == Value) { return null; }
                            citizenbI.LifeHistory = Value;
                            citizenbI.Lifehistory_Lastupdate = updatedTimestamp;
                            entities.SaveChanges();
                            break;
                        }
                    case "sygdomshistorie-content":
                        {
                            if (citizenbI.MedicalHistory == Value) { return null; }
                            citizenbI.MedicalHistory = Value;
                            citizenbI.MedicalHistory_Lastupdate = updatedTimestamp;
                            entities.SaveChanges();
                            break;
                        }
                    case "Skoleoplysninger-content":
                        {
                            if (citizenbI.SchoolInformation == Value) { return null; }
                            citizenbI.SchoolInformation = Value;
                            citizenbI.SchoolInformation_Lastupdate = updatedTimestamp;
                            entities.SaveChanges();
                            break;
                        }
                    case "Socialfaglige-content":
                        {
                            if (citizenbI.SocialInformation == Value) { return null; }
                            citizenbI.SocialInformation = Value;
                            citizenbI.SocialInformation_Lastupdate = updatedTimestamp;
                            entities.SaveChanges();
                            break;
                        }
                    default:
                        break;
                }
                var httpResponseMessage = new HttpResponseMessage();                
                httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject(updatedTimestamp));
                httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                return httpResponseMessage;
            }
            catch
            {
                return null;
            }
        }
    }
    public partial class Citizen_BackgroundInfo
    {
        public System.Guid GUID { get; set; }
        public System.DateTime Createdtimestamp { get; set; }
        public System.DateTime Log_from { get; set; }
        public System.Guid Log_UserId { get; set; }
        public System.Guid CitizenId { get; set; }
        public string LifeHistory { get; set; }
        public Nullable<System.DateTime> Lifehistory_Lastupdate { get; set; }
        public string MedicalHistory { get; set; }
        public Nullable<System.DateTime> MedicalHistory_Lastupdate { get; set; }
        public string HealthInformation { get; set; }
        public Nullable<System.DateTime> HealthInformation_Lastupdate { get; set; }
        public string SchoolInformation { get; set; }
        public Nullable<System.DateTime> SchoolInformation_Lastupdate { get; set; }
        public string SocialInformation { get; set; }
        public Nullable<System.DateTime> SocialInformation_Lastupdate { get; set; }
    }
}
