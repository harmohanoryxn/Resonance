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
    
    public partial class Update
    {
        public int updateId { get; set; }
        public string type { get; set; }
        public string source { get; set; }
        public string value { get; set; }
        public Nullable<System.DateTime> dateCreated { get; set; }
        public Nullable<int> Bed_bedId { get; set; }
        public Nullable<int> Order_orderId { get; set; }
        public Nullable<int> Admission_admissionId { get; set; }
    
        public virtual Admission_tbl Admission { get; set; }
        public virtual Bed Bed { get; set; }
        public virtual Order_tbl Order { get; set; }
    }
}
