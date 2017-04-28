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
    
    public partial class DeviceConfiguration
    {
        public int deviceId { get; set; }
        public int shortcutKeyNo { get; set; }
        public int configurationId { get; set; }
        public int cleaningBedDataTimeout { get; set; }
        public int orderTimeout { get; set; }
        public int presenceTimeout { get; set; }
        public int rfidTimeout { get; set; }
        public int dischargeTimeout { get; set; }
        public int admissionsTimeout { get; set; }
    
        public virtual Configuration Configuration { get; set; }
        public virtual Device Device { get; set; }
    }
}
