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
    
    public partial class Message
    {
        public System.Guid GUID { get; set; }
        public System.DateTime Createdtimestamp { get; set; }
        public System.DateTime Log_from { get; set; }
        public System.Guid Log_UserId { get; set; }
        public System.Guid CitizenId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public byte Status { get; set; }
        public bool Deleted { get; set; }
        public Nullable<System.Guid> recipient { get; set; }
        public string MessageType { get; set; }
        public string MessageStatus { get; set; }
    
        public virtual Security_Citizens Security_Citizens { get; set; }
        public virtual Security_User Security_User { get; set; }
    }
}