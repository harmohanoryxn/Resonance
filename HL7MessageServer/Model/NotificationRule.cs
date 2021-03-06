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
    
    public partial class NotificationRule
    {
        public int notificationRuleId { get; set; }
        public string description { get; set; }
        public int priorToProcedureTime { get; set; }
        public int durationMinutes { get; set; }
        public int radiationRiskDurationMinutes { get; set; }
        public bool isAcknowledgmentRequired { get; set; }
        public int Procedure_procedureId { get; set; }
    
        public virtual Procedure Procedure { get; set; }
    }
}
