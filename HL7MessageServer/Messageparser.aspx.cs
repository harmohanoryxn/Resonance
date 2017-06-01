using H7Message;
using HL7MessageServer.Model;
using NHapi.Base.Parser;
using NHapi.Base.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HL7MessageServer
{
    public partial class Messageparser : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ddlselect();
            }
        }

        protected void btnShow_Click(object sender, EventArgs e)
        {
            string folderPath = ddlfolderselect.SelectedValue;
            WCSHL7Entities wcs = new WCSHL7Entities();
            IList<Gridvalcheck> gridval = new List<Gridvalcheck>();
            DirectoryInfo info = new DirectoryInfo(folderPath);
            FileInfo[] files = info.GetFiles().OrderBy(p => p.CreationTime).ToArray();
            foreach (string file in Directory.EnumerateFiles(folderPath))
            {
                string message = File.ReadAllText(file);
                var parser = new PipeParser();
                var messageParsed = parser.Parse(message);
                Terser tst = new Terser(messageParsed);
                try
                {
                    HLMessageToDB hl7 = new HLMessageToDB();
                    hl7.HL7MessageToDB(message, file);
                    System.IO.File.WriteAllText(ConfigurationManager.AppSettings["Recieved"] + file, message);

                    Gridvalcheck gvd = new Gridvalcheck();
                    var returntype = tst.Get("/MSH-9");

                    string externalPId = tst.Get("/PID-3");
                   


                        if (externalPId == txtMRnumber.Text || Convert.ToString(txtMRnumber.Text)!=null || Convert.ToString(txtMRnumber.Text)!="")
                        {
                            if (returntype == "ORM")
                            {
                                gvd.filename = file;
                                gvd.admissionExtId = externalPId;
                                if (returntype == "ORM")
                                {
                                    gvd.OrderID = tst.Get("/ORC-2");
                                    gvd.DBORDERSTATUS = AdmissionStatusId(tst.Get("/PV1-41"));


                                }
                                else
                                {
                                    gvd.admissionExtId = tst.Get("/PID-18");
                                }
                                gvd.filetime = File.GetLastWriteTime(file);
                                gvd.MessageType = message;
                                string orderNumber = tst.Get("/ORC-2");
                                int orderId = wcs.Order_tbl.Where(c => c.orderNumber == orderNumber).Select(d => d.orderId).FirstOrDefault();
                                gvd.OrderExistsInDB = Convert.ToString(orderId);
                                gridval.Add(gvd);
                            }
                        }
                        else
                    {
                        if (returntype == "ORM")
                        {
                            gvd.filename = file;
                            gvd.admissionExtId = externalPId;
                            if (returntype == "ORM")
                            {
                                gvd.OrderID = tst.Get("/ORC-2");
                                gvd.DBORDERSTATUS = AdmissionStatusId(tst.Get("/PV1-41"));


                            }
                            else
                            {
                                gvd.admissionExtId = tst.Get("/PID-18");
                            }
                            gvd.filetime = File.GetLastWriteTime(file);
                            gvd.MessageType = message;
                            string orderNumber = tst.Get("/ORC-2");
                            int orderId = wcs.Order_tbl.Where(c => c.orderNumber == orderNumber).Select(d => d.orderId).FirstOrDefault();
                            gvd.OrderExistsInDB = Convert.ToString(orderId);
                            gridval.Add(gvd);
                        }
                    }

                    
                   

                }
                catch (Exception)
                {

                }
                GridView1.DataSource = gridval;
                GridView1.DataBind();
            }

        }
        protected void ddlselect()
        {
            string querystring = ConfigurationManager.AppSettings["MessageFolder"]; ;
            var directories = Directory.GetDirectories(querystring);
            ddlfolderselect.DataSource = directories;
            ddlfolderselect.DataBind();
        }
        private string AdmissionStatusId(string status)
        {
            string statusId;
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
            statusId = status;

            return statusId;
        }

        protected void btnUpdateORM_Click(object sender, EventArgs e)
        {
            string folderPath = ddlfolderselect.SelectedValue;
            WCSHL7Entities wcs = new WCSHL7Entities();
            IList<Gridvalcheck> gridval = new List<Gridvalcheck>();
            DirectoryInfo info = new DirectoryInfo(folderPath);
            FileInfo[] files = info.GetFiles().OrderBy(p => p.CreationTime).ToArray();
            foreach (string file in Directory.EnumerateFiles(folderPath))
            {
                string message = File.ReadAllText(file);
                var parser = new PipeParser();
                var messageParsed = parser.Parse(message);
                Terser tst = new Terser(messageParsed);
                Gridvalcheck gvd = new Gridvalcheck();

                try
                {
                    var returntype = tst.Get("/MSH-9");
                    if(returntype=="ORM")
                    {
                       string value= proceduretimeupdate(tst);
                        string externalPId = tst.Get("/PID-3");
                        gvd.filename = file;
                        gvd.admissionExtId = externalPId;
                        if (returntype == "ORM")
                        {
                            gvd.OrderID = tst.Get("/ORC-2");
                            gvd.DBORDERSTATUS = AdmissionStatusId(tst.Get("/PV1-41"));


                        }
                        else
                        {
                            gvd.admissionExtId = tst.Get("/PID-18");
                        }
                        gvd.filetime = File.GetLastWriteTime(file);
                        gvd.MessageType = value;
                        gridval.Add(gvd);
                    }
                }
                catch (Exception ex)
                {

                }

            }
            GridView1.DataSource = gridval;
            GridView1.DataBind();
        }
        public string proceduretimeupdate(Terser tst)
        {
            WCSHL7Entities wcs = new WCSHL7Entities();
            string returnval = "";
            try
            {

                string orderNumber = tst.Get("/ORC-2");
                int orderId = wcs.Order_tbl.Where(c => c.orderNumber == orderNumber).Select(d => d.orderId).FirstOrDefault();
                if (orderId > 0)
                {
                    string admextid = tst.Get("/PID-18");
                    int admissiontypeid = AdmissionType.AdmissionTypeId(tst);
                    int admissionId = Convert.ToInt32(wcs.Admission_tbl.Where(adm => adm.externalId == admextid).Select(ad1 => ad1.admissionId).FirstOrDefault());
                    var ProcedureTime = "";
                    string proceduretimecheck1 = tst.Get("/OBR-27-4");
                    if (proceduretimecheck1 != null || proceduretimecheck1 != "")
                    {
                        if (proceduretimecheck1.Length > 8)
                        {
                            ProcedureTime = DateTime.ParseExact(proceduretimecheck1, "yyyyMMddHHmm", null).ToString("yyyy-MM-dd HH:mm");
                        }
                        else if (proceduretimecheck1.Length == 8)
                        {
                            ProcedureTime = DateTime.ParseExact(proceduretimecheck1, "yyyyMMdd", null).ToString("yyyy-MM-dd HH:mm");
                        }



                    }
                    Order_tbl ad = wcs.Order_tbl.First(c => c.orderNumber == orderNumber && (c.admissionId == admissionId));
                    if (admissiontypeid == 1)
                    {
                        var procedureupdatetimecheck = wcs.Updates.Where(c => c.Order_orderId == ad.orderId && c.type == "Procdure Time Updated").FirstOrDefault();
                        if (procedureupdatetimecheck != null)
                        {
                            DateTime datecreated = Convert.ToDateTime(procedureupdatetimecheck.dateCreated);
                            DateTime updatedtime = Convert.ToDateTime(datecreated.ToShortDateString() + procedureupdatetimecheck.value);
                            DateTime hl7PRoceduredatetime = Convert.ToDateTime(ProcedureTime);
                            if (updatedtime < hl7PRoceduredatetime)
                            {
                                TimeSpan ts = new TimeSpan(00, 00, 00);
                                DateTime dt = Convert.ToDateTime(Convert.ToDateTime(ProcedureTime).Date + ts);
                                ad.procedureTime = dt;
                                returnval = "Procedure Time Updated";
                            }
                            else
                            {
                                returnval = "Procedure Time Already Updated";
                            }
                        }
                        else
                        {
                            string proceduredb = Convert.ToString(ad.procedureTime);

                            if (proceduredb != null || proceduredb != "")
                            {
                                DateTime DBdatetime = Convert.ToDateTime(ad.procedureTime);
                                DateTime hl7messagedatetime = Convert.ToDateTime(ProcedureTime);
                                if (hl7messagedatetime > DBdatetime)
                                {
                                    TimeSpan ts = new TimeSpan(00, 00, 00);
                                    DateTime dt = Convert.ToDateTime(Convert.ToDateTime(ProcedureTime).Date + ts);
                                    ad.procedureTime = dt;
                                }
                            }
                            else
                            {
                                returnval = "Procedure Time as TBA";
                            }

                            
                        }
                    }
                    else
                    {
                        ad.procedureTime = Convert.ToDateTime(ProcedureTime);
                        returnval = "Procedure Time Already Updated";
                    }

                    wcs.Order_tbl.Attach(ad);
                    wcs.Entry(ad).State = EntityState.Modified;
                    wcs.SaveChanges();
                    returnval+=" Result: Success";
                    

                }
                else
                {
                    returnval += "Order Doesnt exists";
                }
            }
            catch (Exception ex)
            {
                returnval+= Convert.ToString(ex.InnerException);
            }
            return returnval;
        }
    }

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
        public DateTime filetime { get; set; }

    }

