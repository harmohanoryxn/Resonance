//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace H7Message
{
    using System;
    using System.Collections.Generic;
    
    public partial class BedCleaningEvent
    {
        public int bedCleaningEventId { get; set; }
        public System.DateTime timestamp { get; set; }
        public int Bed_bedId { get; set; }
        public int bedCleaningEventTypeId { get; set; }
    
        public virtual Bed Bed { get; set; }
        public virtual BedCleaningEventType BedCleaningEventType { get; set; }
    }
}