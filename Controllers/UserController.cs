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
    public class UserController : ApiController
    {
        private KKSOFDBEntities db = new KKSOFDBEntities();

        // GET api/User
        public HttpResponseMessage GetSecurity_User()
        {
            try
            {

                var users = (from p in db.Security_User 
                             orderby p.UserId  
                                 select new
                                 {
                                    UserName = p.Firstname + " " + p.Lastname + " (" + p.UserId +")",
                                    UserId = p.GUID,
                                    UserType = p.Usertype
                                 }).ToList();


                var httpResponseMessage = new HttpResponseMessage();
                httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject(users));
                httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                return httpResponseMessage;
            }
            catch
            {
                return null;
            }
        }

        // GET api/User/5
        [ResponseType(typeof(Security_User))]
        public HttpResponseMessage GetSecurity_User(Guid id)
        {
           
                try
                {
                    var httpResponseMessage = new HttpResponseMessage();
                    var user = from p in db.Security_User
                               where p.GUID == id
                               select new
                               {
                                   Firstname = p.Firstname,
                                   Lastname = p.Lastname,
                                   Profession = p.Profession,
                                   UserName = p.UserId
                               };
                    if (user != null)
                    {

                        httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject(user));
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

        [HttpGet]
        [Route("api/user/validate")]
        public HttpResponseMessage ValidateUser(string username, string password)
        {
            try
            {
                var httpResponseMessage = new HttpResponseMessage();
                var user = db.Security_User.FirstOrDefault(x => x.UserId.ToLower() == username.ToLower() && x.Password == password);
                if (user != null)
                {                    
                    var selectedUser = from p in db.Security_User
                                        where p.GUID == user.GUID
                                       select new
                                       {
                                           GUID = p.GUID,
                                           Firstname = p.Firstname,
                                           Lastname = p.Lastname,
                                           Profession = p.Profession,
                                           UserName = p.UserId,
                                           Usertype = p.Usertype
                                       };

                    httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject(selectedUser));
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool Security_UserExists(Guid id)
        {
            return db.Security_User.Count(e => e.GUID == id) > 0;
        }
    }
}