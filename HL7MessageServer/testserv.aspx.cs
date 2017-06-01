using H7Message;
using HL7MessageServer.Classes;
using HL7MessageServer.Helpers;
using HL7MessageServer.Model;
using NHapi.Base.Parser;
using NHapi.Base.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
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
            if (!IsPostBack)
            {
                ddlselect();
            }
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
        public class Gridvalcheck
        {
            public string filename { get; set; }
            public string MessageType { get; set; }
            public string OrderID { get; set; }
            public string admissionId { get; set; }
            public string admissionExtId { get; set; }
            public string DepLoc { get; set; }
            public string deplocpv { get; set; }
            public string admtype { get; set; }

            public string OrderExistsInDB { get; set; }
            public string FileOrderStatus { get; set; }
            public string DBORDERSTATUS { get; set; }
            public string filetime { get; set; }

        }

        protected void btnparsemessages_Click(object sender, EventArgs e)
        {
            string folderPath = ddlfolderselect.SelectedValue;
            WCSHL7Entities wcs = new WCSHL7Entities();
            IList<Gridvalcheck> gridval = new List<Gridvalcheck>();
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


                    Gridvalcheck gvd = new Gridvalcheck();
                    gvd.filename = file;
                    gvd.MessageType = tst.Get("/MSH-9");
                    gvd.DepLoc = assignedpatientlocation;
                    if (returntype == ddlval.SelectedValue)
                    {
                        if (returntype == "ORM")
                        {
                            gvd.deplocpv = tst.Get("/OBR-18");
                            gvd.OrderID = tst.Get("/ORC-2");
                            string orderid = tst.Get("/ORC-2");
                            int dborderid = wcs.Order_tbl.Where(c => c.externalId == orderid).Select(d => d.orderId).FirstOrDefault();
                            if (dborderid > 0)
                            {
                                gvd.OrderExistsInDB = "Yes";

                            }
                            else
                            {
                                int admcheck = wcs.Admission_tbl.Where(c => c.externalId == admextid).Select(d => d.admissionId).FirstOrDefault();
                                gvd.admissionId = Convert.ToString(admcheck);

                                gvd.admissionExtId = admextid;
                                if (admcheck > 0)
                                {
                                    HLMessageToDB hl7 = new HLMessageToDB();
                                    hl7.HL7MessageToDB(message, file);
                                    dborderid = wcs.Order_tbl.Where(c => c.externalId == orderid).Select(d => d.orderId).FirstOrDefault();
                                    if (dborderid > 0)
                                        gvd.OrderExistsInDB = "Yes";
                                    else
                                        gvd.OrderExistsInDB = "Retry Failure";
                                }
                                else
                                {
                                    gvd.OrderExistsInDB = "No";
                                }

                            }
                            int dborderStatusid = wcs.Order_tbl.Where(c => c.externalId == orderid).Select(d => d.orderStatusId).FirstOrDefault();
                            if (dborderStatusid > 0)
                            {
                                gvd.DBORDERSTATUS = wcs.OrderStatus.Where(c => c.orderStatusId == dborderStatusid).Select(d => d.status).FirstOrDefault();
                            }
                            string status = tst.Get("/ORC-5");
                            switch (status)
                            {
                                case "L":
                                    status = "InProgress";
                                    break;
                                case "I":
                                    status = "InProgress";
                                    break;
                                case "T":
                                    status = "InProgress";
                                    break;
                                case "C":
                                    status = "Completed";
                                    break;
                                case "R":
                                    status = "Completed";
                                    break;
                                case "X":
                                    status = "Cancelled";
                                    break;
                            }
                            gvd.FileOrderStatus = status;


                        }
                        else
                        {

                            int admcheck = wcs.Admission_tbl.Where(c => c.externalId == admextid).Select(d => d.admissionId).FirstOrDefault();
                            if (admcheck == 0)
                            {
                                HLMessageToDB hl7 = new HLMessageToDB();
                                hl7.HL7MessageToDB(message, file);
                                admcheck = wcs.Admission_tbl.Where(c => c.externalId == admextid).Select(d => d.admissionId).FirstOrDefault();
                            }
                            int admissionstatsu = AdmissionStatusId(tst.Get("/PV1-41"));
                            if (admissionstatsu == 0)
                            {
                                gvd.OrderID = tst.Get("/PV1-41");
                            }

                            gvd.admissionId = Convert.ToString(admcheck);
                            gvd.admissionExtId = tst.Get("/PID-18");
                        }

                        gridval.Add(gvd);
                    }


                }
                catch (Exception ex)
                {

                    string innerexception = Convert.ToString(ex.InnerException);
                    HL7messageToFile.Exceptionhandler(ex.Message + "Internal error" + innerexception, Convert.ToString(ex.StackTrace) + "File name:" + file);
                }
            }
            totallit.Text = "Total In DB: " + gridval.Where(c => c.OrderExistsInDB == "Yes").Count() + " Total:" + gridval.Count() + " Total Not in DB: " + gridval.Where(d => d.OrderExistsInDB == "No").Count();
            GridView1.DataSource = gridval;
            GridView1.DataBind();

        }
        protected void ddlselect()
        {
            string querystring = ConfigurationManager.AppSettings["MessageFolder"]; ;
            var directories = Directory.GetDirectories(querystring);
            ddlfolderselect.DataSource = directories;
            ddlfolderselect.DataBind();
        }
        private int AdmissionStatusId(string status)
        {
            int statusId;
            WCSHL7Entities wcs = new WCSHL7Entities();
            switch (status)
            {
                case "SCH":
                    status = "Registered";
                    break;
                case "PRE":
                    status = "Registered";
                    break;
                case "ADM":
                    status = "Admitted";
                    break;
                case "REG":
                    status = "Registered";
                    break;
                case "DIS":
                    status = "Discharged";
                    break;
                case "DEP":
                    status = "Discharged";
                    break;
            }
            statusId = Convert.ToInt32(wcs.tbl_AdmissionStatus.Where(ast => ast.status == status).Select(admst => admst.admissionStatusId).FirstOrDefault());

            return statusId;
        }

        protected void btnBEdIdUpdate_Click(object sender, EventArgs e)
        {
            string folderPath = ddlfolderselect.SelectedValue;
            WCSHL7Entities wcs = new WCSHL7Entities();
            IList<Gridvalcheck> gridval = new List<Gridvalcheck>();
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


                    Gridvalcheck gvd = new Gridvalcheck();
                    gvd.filename = file;
                    
                    if (returntype == "ADT")
                    {
                        string admexternalId = tst.Get("/PID-18");
                        Admission_tbl ad = wcs.Admission_tbl.First(i => i.externalId == admexternalId);
                        if (ad != null)
                        {
                            ///////////Patient Bed ID////////////////////
                            int patientBedid = PatientBed.PBed(tst.Get("/PV1-3-3"), tst.Get("/PV1-3-2"));
                            if (patientBedid > 0)
                            {
                                ad.Bed_bedId = patientBedid;
                            }
                            gvd.admissionId = Convert.ToString(patientBedid);
                        }
                        gvd.MessageType = tst.Get("/PV1-3-3");
                        gvd.DepLoc = tst.Get("/PV1-3-2");

                       
                    }
                    gridval.Add(gvd);

                }
                catch (Exception ex)
                {

                }
            }
            GridView1.DataSource = gridval;
            GridView1.DataBind();
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            totallit.Text = IsServerConnected("data source=.;initial catalog=WCS;integrated security=True;MultipleActiveResultSets=True;");
        }
        private static string IsServerConnected(string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    return "Connected";
                }
                catch (SqlException ex)
                {
                    return ex.Message;
                }
            }
        }
    }
}