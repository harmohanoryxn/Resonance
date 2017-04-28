using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace HL7MessageServer
{
    public static class Hl7Filereader
    {
        public static string ReadContent(string FileName)
        {
            string HL7Message = "";
            try
            {
                string directory = ConfigurationManager.AppSettings["MessageFolder"];
                HL7Message = File.ReadAllText(@"" + directory + "\\" + FileName, Encoding.UTF8);
                return HL7Message;
            }
            catch (Exception ex)
            {
                HL7Message = "Error Logged";
                return HL7Message;
            }
        }
    }
}