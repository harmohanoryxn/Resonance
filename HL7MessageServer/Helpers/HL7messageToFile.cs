using HL7MessageServer.ErrorHandler;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace H7Message
{
    public static class HL7messageToFile
    {
        public static string writecontent(string message,string messagetype,string mrcode)
        {
            try
            {
                Guid gid = Guid.NewGuid();
                string currentdateguid = DateTime.Now.ToString("ddMMyyyyHHmm")+gid;
                string MessageRootDirectorty = ConfigurationManager.AppSettings["MessageFolder"];
                string directoryname = "";
                var currentdate = DateTime.Now.ToString("ddMMyyyy");
                if (Convert.ToString(mrcode) == null || Convert.ToString(mrcode) == "")
                {
                    directoryname = MessageRootDirectorty +  messagetype + currentdate + "\\Unknown";
                }
                else
                {
                    directoryname = MessageRootDirectorty +  messagetype + "\\" + currentdate + "\\" + mrcode;
                }

                var directory = System.IO.Directory.CreateDirectory(directoryname);
                string Recievedfilename = directoryname + "\\" + currentdateguid + ".txt";
                System.IO.File.WriteAllText(Recievedfilename, message);
                return currentdateguid;
            }
            catch(Exception ex)
            {
                ErrorMessage err = new ErrorMessage();
                err.Innermessage = Convert.ToString(ex.InnerException);
                err.StackTrace = ex.StackTrace;
                err.Ordernumber = "Unable to generate file";
                err.AdmissionNumber = "Unable to generate file";
                err.EntityError = message;
                err.FileName = "";
                err.Messagetype = "Unknown";
                err.ErrorDatetime = DateTime.Now.ToShortDateString();
                err.MRCode = "NA";
                XMLCreator.xmlwriter(err);
                return "100";
            }
        }
        public static void Exceptionhandler(string message,string innerexception)
        {
            Guid gid = Guid.NewGuid();
            string combinedmessage = message + "   Inner Exception :" + innerexception;
            string directory = ConfigurationManager.AppSettings["ErrorFolder"];
            string currentdate = DateTime.Now.ToString("ddMMyyyyHHmm")+gid;
            System.IO.File.WriteAllText(directory + currentdate.ToString() + ".txt", combinedmessage);
        }
    }
}
