using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

using System.Xml.Serialization;

namespace HL7MessageServer.ErrorHandler
{
    public static class XMLCreator
    {
        public static string xmlwriter(ErrorMessage message)
        {
            try
            {
                string ErrorRootDirectorty = ConfigurationManager.AppSettings["ErrorFolder"];
                string directoryname = "";
                var currentdate = DateTime.Now.Date;
                if (Convert.ToString(message.MRCode) == null || Convert.ToString(message.MRCode) =="")
                {
                    directoryname = ErrorRootDirectorty + "\\" + message.Messagetype + currentdate + "\\Unknown";
                }
                else
                {
                    directoryname = ErrorRootDirectorty + "\\" + message.Messagetype + "\\" + currentdate + "\\" + message.MRCode ;
                }

                var directory = System.IO.Directory.CreateDirectory(directoryname);
                string errorfilename = directoryname + "\\" + message.FileName + ".xml";
                //create the serialiser to create the xml
                XmlSerializer serialiser = new XmlSerializer(typeof(List<ErrorMessage>));

                // Create the TextWriter for the serialiser to use
                TextWriter Filestream = new StreamWriter(directoryname);

                //write to the file
                serialiser.Serialize(Filestream, message);

                // Close the file
                Filestream.Close();
                return "Error File Created";
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }
    }
}