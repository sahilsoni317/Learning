//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KKSOFDemoApp.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Security_User
    {
        public Security_User()
        {
            this.Citizen_FollowUps = new HashSet<Citizen_FollowUps>();
            this.Citizen_Notes = new HashSet<Citizen_Notes>();
            this.Citizen_BackgroundInformation = new HashSet<Citizen_BackgroundInformation>();
            this.Deadlines = new HashSet<Deadline>();
            this.Deadlines1 = new HashSet<Deadline>();
            this.Messages = new HashSet<Message>();
            this.Documents = new HashSet<Document>();
            this.CitizenstatusNoteMappings = new HashSet<CitizenstatusNoteMapping>();
        }
    
        public System.Guid GUID { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public System.DateTime Createdtimestamp { get; set; }
        public byte Usertype { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Profession { get; set; }
    
        public virtual ICollection<Citizen_FollowUps> Citizen_FollowUps { get; set; }
        public virtual ICollection<Citizen_Notes> Citizen_Notes { get; set; }
        public virtual ICollection<Citizen_BackgroundInformation> Citizen_BackgroundInformation { get; set; }
        public virtual ICollection<Deadline> Deadlines { get; set; }
        public virtual ICollection<Deadline> Deadlines1 { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<Document> Documents { get; set; }
        public virtual ICollection<CitizenstatusNoteMapping> CitizenstatusNoteMappings { get; set; }
    }
}
