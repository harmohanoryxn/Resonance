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
    
    public partial class Order_tbl
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order_tbl()
        {
            this.Notes = new HashSet<Note>();
            this.Notifications = new HashSet<Notification>();
            this.Updates = new HashSet<Update>();
        }
    
        public int orderId { get; set; }
        public int externalSourceId { get; set; }
        public string externalId { get; set; }
        public string orderNumber { get; set; }
        public Nullable<System.DateTime> procedureTime { get; set; }
        public int orderStatusId { get; set; }
        public Nullable<System.DateTime> completedTime { get; set; }
        public int admissionId { get; set; }
        public string clinicalIndicator { get; set; }
        public Nullable<int> estimatedProcedureDuration { get; set; }
        public int Procedure_procedureId { get; set; }
        public int Department_locationId { get; set; }
        public Nullable<int> OrderingDoctor_doctorId { get; set; }
        public bool isHidden { get; set; }
        public bool acknowledged { get; set; }
        public Nullable<System.DateTime> acknowledgedTime { get; set; }
        public string acknowledgedBy { get; set; }
        public string history { get; set; }
        public string diagnosis { get; set; }
        public string currentCardiologist { get; set; }
        public bool requiresSupervision { get; set; }
        public bool requiresFootwear { get; set; }
        public bool requiresMedicalRecords { get; set; }
    
        public virtual Admission_tbl Admission { get; set; }
        public virtual Doctor Doctor { get; set; }
        public virtual ExternalSource ExternalSource { get; set; }
        public virtual Location Location { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Note> Notes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual Procedure Procedure { get; set; }
        public virtual OrderStatu OrderStatu { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Update> Updates { get; set; }
    }
}
