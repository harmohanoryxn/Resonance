using H7Message;
using HL7MessageServer.Classes;
using HL7MessageServer.Model;
using NHapi.Base.Parser;
using NHapi.Base.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace HL7MessageServer
{
    public partial class testserv : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btn_Click(object sender, EventArgs e)
        {
            string folderPath = ConfigurationManager.AppSettings["TestServ"];
            IList<Gridval> gridval = new List<Gridval>();
            DirectoryInfo info = new DirectoryInfo(folderPath);
            FileInfo[] files = info.GetFiles().OrderBy(p => p.CreationTime).ToArray();
            foreach (string file in Directory.EnumerateFiles(folderPath))
            {
                string contents = File.ReadAllText(file);
                if (contents != "Error Logged")
                {
                    //HLMessageToDB hl7 = new HLMessageToDB();
                    //hl7.HL7MessageToDB(contents);
                    //System.IO.File.WriteAllText(ConfigurationManager.AppSettings["Recieved"]+DateTime.Now+".txt" , contents);

                }
                else
                {
                    System.IO.File.WriteAllText(ConfigurationManager.AppSettings["ErrorFolder"], "error");
                }
                var parser = new PipeParser();

                var messageParsed = parser.Parse(contents);
                Terser tst = new Terser(messageParsed);
                var MessageType = tst.Get("/MSH-9");
                string orderNumber = "";
                string Department_location = "";
                string deplocpv = "";
                string admtype = "";
                string givenname = "";
                if (MessageType == "ORM")
                {
                    orderNumber = tst.Get("/ORC-2");
                    Department_location = tst.Get("/ORC-13");
                    deplocpv = "ORC-13";
                    if (Department_location == "" || Department_location == null)
                    {
                        Department_location = tst.Get("/PV1-3");
                        deplocpv = "PV1-3";
                    }
                }
                else if (MessageType == "ADM")
                {
                    orderNumber = tst.Get("/PID-3");
                    admtype = tst.Get("/PV1-2");
                    givenname = tst.Get("/PID-5-2");
                }

                Gridval gv = new Gridval();
                gv.filename = file;
                gv.MessageType = MessageType;
                gv.OrderID = orderNumber;
                gv.DepLoc = Department_location;
                gv.deplocpv = deplocpv;
                gv.admtype = admtype;
                gridval.Add(gv);
            }
            GridView1.DataSource = gridval;
            GridView1.DataBind();
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string folderPath = ConfigurationManager.AppSettings["TestServ"];
            IList<Gridval> gridval = new List<Gridval>();
            DirectoryInfo info = new DirectoryInfo(folderPath);
            FileInfo[] files = info.GetFiles().OrderBy(p => p.CreationTime).ToArray();
            foreach (string file in Directory.EnumerateFiles(folderPath))
            {
                string message = File.ReadAllText(file);
                try
                {
                    var parser = new PipeParser();
                    var messageParsed = parser.Parse(message);
                    Terser tst = new Terser(messageParsed);
                    var MessageType = tst.Get("/MSH-9");
                    var returntype = "";



                    switch (MessageType)
                    {

                        case "ADT":
                            returntype = adtmessage(tst);
                            break;
                            //case "ORU":
                            //    oruMessage(tst);
                            //    break;



                    }
                    Gridval gv = new Gridval();
                    gv.filename = file;
                    gv.MessageType = MessageType;
                    gv.OrderID = returntype;

                    gridval.Add(gv);
                }
                catch (Exception ve)
                {
                    Gridval gv = new Gridval();
                    gv.filename = file;
                    gv.MessageType = Convert.ToString(ve.InnerException);
                    gv.OrderID = ve.Message;

                    gridval.Add(gv);
                    HL7messageToFile.Exceptionhandler(Convert.ToString(ve.InnerException), ve.Message);
                }

            }
            GridView1.DataSource = gridval;
            GridView1.DataBind();
        }
        public string adtmessage(Terser tst)
        {
            WCSHL7Entities wcs = new WCSHL7Entities();
            var messageevent = tst.Get("/MSH-9-2");
            try
            {
                string extPID = tst.Get("/PID-2");
                int check = wcs.Patient_tbl.Where(c => c.externalId == extPID).Select(d => d.patientId).FirstOrDefault();
                if (check > 0)
                {
                    Patient_tbl pd = wcs.Patient_tbl.First(p => p.externalId == extPID);
                   

                    if (messageevent == "A01" || messageevent == "A04")
                    {
                        string extPID1 = tst.Get("/PID-3");
                        pd.externalId = extPID1;
                    }
                    wcs.SaveChanges();
                }
                return "Added";
            }
            catch (Exception ve)
            {
                HL7messageToFile.Exceptionhandler(Convert.ToString(ve.InnerException), ve.Message);
                return "Not Added";
            }


        }

        protected void btn1_Click(object sender, EventArgs e)
        {
            string folderPath = ConfigurationManager.AppSettings["xml"];
            IList<Gridval> gridval = new List<Gridval>();
            DirectoryInfo info = new DirectoryInfo(folderPath);
            FileInfo[] files = info.GetFiles().OrderBy(p => p.CreationTime).ToArray();
            foreach (string file in Directory.EnumerateFiles(folderPath))
            {
                string message = File.ReadAllText(file);
                try
                {
                    XmlDocument xml = HL7ToXmlConverter.ConvertToXml(message);
                }
                catch (Exception ex)
                {

                }
            }
        }
        public class Gridval
        {
            public string filename { get; set; }
            public string MessageType { get; set; }
            public string OrderID { get; set; }
            public string admissionId { get; set; }
            public string DepLoc { get; set; }
            public string deplocpv { get; set; }
            public string admtype { get; set; }
            public string givenname { get; set; }

        }

        protected void btnparsemessages_Click(object sender, EventArgs e)
        {
            string folderPath = ConfigurationManager.AppSettings["TestServ"];
            IList<Gridval> gridval = new List<Gridval>();
            DirectoryInfo info = new DirectoryInfo(folderPath);
            FileInfo[] files = info.GetFiles().OrderBy(p => p.CreationTime).ToArray();
            foreach (string file in Directory.EnumerateFiles(folderPath))
            {
                string message = File.ReadAllText(file);
                try
                {
                    var parser = new PipeParser();
                    var messageParsed = parser.Parse(message);
                    Terser tst = new Terser(messageParsed);
                    var admextid = tst.Get("/PID-18");
                    string assignedpatientlocation = tst.Get("/PV1-3");
                    var returntype = tst.Get("/MSH-9");
                   
                       
                        Gridval gvd = new Gridval();
                        gvd.filename = file;
                        gvd.MessageType = tst.Get("/MSH-9");
                        gvd.DepLoc = assignedpatientlocation;
                        
                     if(returntype=="ORM")
                    {
                        gvd.deplocpv = tst.Get("/OBR-18");
                        gvd.OrderID = tst.Get("/ORC-2");
                    }
                    else
                    {
                        gvd.admissionId = tst.Get("/PID-18");
                    }
                        
                        gridval.Add(gvd);
                  
                   
                }
                catch (Exception ex)
                {
                   
                    string innerexception = Convert.ToString(ex.InnerException);
                    HL7messageToFile.Exceptionhandler(ex.Message + "Internal error" + innerexception, Convert.ToString(ex.StackTrace));
                }
            }
            GridView1.DataSource = gridval;
            GridView1.DataBind();

        }
    }
}