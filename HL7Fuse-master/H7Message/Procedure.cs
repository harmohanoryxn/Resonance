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
    
    public partial class Procedure
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Procedure()
        {
            this.NotificationRules = new HashSet<NotificationRule>();
            this.Orders = new HashSet<Order_tbl>();
        }
    
        public int procedureId { get; set; }
        public int externalSourceId { get; set; }
        public string externalId { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public Nullable<int> durationMinutes { get; set; }
        public int ProcedureCategory_procedureCategoryId { get; set; }
    
        public virtual ExternalSource ExternalSource { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NotificationRule> NotificationRules { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order_tbl> Orders { get; set; }
        public virtual ProcedureCategory ProcedureCategory { get; set; }
    }
}
