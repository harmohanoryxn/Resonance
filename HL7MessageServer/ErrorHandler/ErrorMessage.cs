using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace HL7MessageServer.ErrorHandler
{
    public  class ErrorMessage
    {
        [XmlElement("MRCode")]
        public string MRCode { get; set; }
        [XmlElement("Messagetype")]
        public string Messagetype { get; set; }
        [XmlElement("Ordernumber")]
        public string Ordernumber { get; set; }
        [XmlElement("AdmissionNumber")]
        public string AdmissionNumber { get; set; }
        [XmlElement("Innermessage")]
        public string Innermessage { get; set; }
        [XmlElement("StackTrace")]
        public string StackTrace { get; set; }
        [XmlElement("EntityError")]
        public string EntityError { get; set; }
        [XmlElement("FileName")]
        public string FileName { get; set; }
        [XmlElement("MessageDatetime")]
        public string MessageDatetime { get; set; }
        [XmlElement("ErrorDatetime")]
        public string ErrorDatetime { get; set; }

    }
}