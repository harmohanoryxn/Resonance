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
                return "error";
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
