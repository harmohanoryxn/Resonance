using H7Message;
using HL7MessageServer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HL7MessageServer
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string url = Request.RawUrl;
            if (url.Contains('?'))
            {
                string querystring = Request.QueryString["filename"];
                string HL7Message = Hl7Filereader.ReadContent(querystring);
                if (HL7Message != "Error Logged")
                {
                    HLMessageToDB hl7 = new HLMessageToDB();
                    hl7.HL7MessageToDB(HL7Message);
                    System.IO.File.WriteAllText(ConfigurationManager.AppSettings["Recieved"] + querystring, HL7Message);

                }
                else
                {
                    System.IO.File.WriteAllText(ConfigurationManager.AppSettings["ErrorFolder"] + querystring, "error");
                }
            }

        }
    }
}