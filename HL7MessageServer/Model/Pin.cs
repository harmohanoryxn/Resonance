//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HL7MessageServer.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Pin
    {
        public int pinId { get; set; }
        public string pin1 { get; set; }
        public int Device_deviceId { get; set; }
    
        public virtual Device Device { get; set; }
    }
}
