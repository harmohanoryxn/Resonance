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
    
    public partial class BedCleaningEventType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BedCleaningEventType()
        {
            this.BedCleaningEvents = new HashSet<BedCleaningEvent>();
        }
    
        public int bedCleaningEventTypeId { get; set; }
        public string eventType { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BedCleaningEvent> BedCleaningEvents { get; set; }
    }
}
