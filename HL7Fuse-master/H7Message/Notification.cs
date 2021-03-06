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
    
    public partial class Notification
    {
        public int notificationId { get; set; }
        public int notificationTypeId { get; set; }
        public string description { get; set; }
        public int priorToProcedureTime { get; set; }
        public bool isAcknowledgmentRequired { get; set; }
        public bool acknowledged { get; set; }
        public Nullable<System.DateTime> acknowledgedTime { get; set; }
        public string acknowledgedBy { get; set; }
        public Nullable<int> notificationOrder { get; set; }
        public int orderId { get; set; }
        public int durationMinutes { get; set; }
        public int radiationRiskDurationMinutes { get; set; }
    
        public virtual NotificationType NotificationType { get; set; }
        public virtual Order_tbl Order { get; set; }
    }
}
