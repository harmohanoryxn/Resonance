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
    
    public partial class ConfigurationLocation
    {
        public int configurationLocationId { get; set; }
        public int configurationId { get; set; }
        public int locationId { get; set; }
        public bool isDefault { get; set; }
    
        public virtual Configuration Configuration { get; set; }
        public virtual Location Location { get; set; }
    }
}
