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
    
    public partial class Citizen_BackgroundInformation
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
    
        public virtual Security_Citizens Security_Citizens { get; set; }
        public virtual Security_User Security_User { get; set; }
    }
}
