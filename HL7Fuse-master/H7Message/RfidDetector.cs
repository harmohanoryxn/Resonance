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
    
    public partial class RfidDetector
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RfidDetector()
        {
            this.RfidDetections = new HashSet<RfidDetection>();
        }
    
        public int rfidDetectorId { get; set; }
        public int externalSourceId { get; set; }
        public string externalId { get; set; }
        public Nullable<int> Location_locationId { get; set; }
        public Nullable<int> WaitingArea_waitingAreaId { get; set; }
    
        public virtual ExternalSource ExternalSource { get; set; }
        public virtual Location Location { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RfidDetection> RfidDetections { get; set; }
        public virtual WaitingArea WaitingArea { get; set; }
    }
}
