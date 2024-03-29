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
    
    public partial class Citizen_Notes
    {
        public Citizen_Notes()
        {
            this.Citizen_FollowUps = new HashSet<Citizen_FollowUps>();
            this.CitizenstatusNoteMappings = new HashSet<CitizenstatusNoteMapping>();
        }
    
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
    
        public virtual ICollection<Citizen_FollowUps> Citizen_FollowUps { get; set; }
        public virtual Security_Citizens Security_Citizens { get; set; }
        public virtual Security_User Security_User { get; set; }
        public virtual Citizen_FollowUps Citizen_FollowUps1 { get; set; }
        public virtual ICollection<CitizenstatusNoteMapping> CitizenstatusNoteMappings { get; set; }
    }
}
