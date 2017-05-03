using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace H7Message
{
    public static class HL7messageToFile
    {
        public static string writecontent(string message)
        {
            try
            {
                Guid gid= Guid.NewGuid(); 
                string currentdate = DateTime.Now.ToString("ddMMyyyyHHmm")+gid;
                string directory = ConfigurationManager.AppSettings["MessageFolder"];
                System.IO.File.WriteAllText(directory + currentdate.ToString()+".txt", message);
                return currentdate.ToString() + ".txt";
            }
            catch(Exception ex)
            {
                return "error";
            }
        }
        public static void Exceptionhandler(string message,string innerexception)
        {
            string combinedmessage = message + "   Inner Exception :" + innerexception;
            Guid gid = Guid.NewGuid();
            string directory = ConfigurationManager.AppSettings["ErrorFolder"];
            string currentdate = DateTime.Now.ToString("ddMMyyyyHHmm")+gid;
            System.IO.File.WriteAllText(directory + currentdate.ToString() + ".txt", combinedmessage);
        }
    }
}
