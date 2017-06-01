using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HL7MessageServer.ErrorHandler
{
    public  class ErrorMessage
    {
        public string MRCode { get; set; }
        public string Messagetype { get; set; }
        public string Ordernumber { get; set; }

        public string AdmissionNumber { get; set; }
        public string Innermessage { get; set; }
        public string StackTrace { get; set; }
        public string EntityError { get; set; }
        public string FileName { get; set; }
        public string MessageDatetime { get; set; }
        public string ErrorDatetime { get; set; }

    }
}