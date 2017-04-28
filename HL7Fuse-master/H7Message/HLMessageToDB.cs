using H7Message;
using NHapi.Base.Model;
using NHapi.Base.Parser;
using NHapi.Base.Util;
using NHapi.Model.V23.Message;
using NHapi.Model.V24.Segment;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Data.Entity.Migrations;

namespace H7Message
{
    public class HLMessageToDB
    {
        
        public void HL7MessageToDB(string message)
        {
            
            var MessageType ="";
            string filename =HL7messageToFile.writecontent(message);
            string value = HL7ToApi.consume(filename, MessageType);
           
            

        }
        
      
       
    }
}
