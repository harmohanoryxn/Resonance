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
    
    public partial class Admission_tbl
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Admission_tbl()
        {
            this.Orders = new HashSet<Order_tbl>();
            this.Updates = new HashSet<Update>();
        }
    
        public int admissionId { get; set; }
        public int externalSourceId { get; set; }
        public string externalId { get; set; }
        public Nullable<System.DateTime> admitDateTime { get; set; }
        public Nullable<System.DateTime> estimatedDischargeDateTime { get; set; }
        public Nullable<System.DateTime> dischargeDateTime { get; set; }
        public int patientId { get; set; }
        public Nullable<int> PrimaryCareDoctor_doctorId { get; set; }
        public Nullable<int> AttendingDoctor_doctorId { get; set; }
        public Nullable<int> AdmittingDoctor_doctorId { get; set; }
        public int AdmissionType_admissionTypeId { get; set; }
        public int AdmissionStatus_admissionStatusId { get; set; }
        public int Location_locationId { get; set; }
        public Nullable<int> Bed_bedId { get; set; }
    
        public virtual tbl_AdmissionStatus AdmissionStatu { get; set; }
        public virtual tbl_AdmissionType AdmissionType { get; set; }
        public virtual Doctor Doctor { get; set; }
        public virtual Doctor Doctor1 { get; set; }
        public virtual Bed Bed { get; set; }
        public virtual Location Location { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order_tbl> Orders { get; set; }
        public virtual Doctor Doctor2 { get; set; }
        public virtual ExternalSource ExternalSource { get; set; }
        public virtual Patient_tbl Patient { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Update> Updates { get; set; }
    }
}
