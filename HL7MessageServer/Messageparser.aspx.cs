using H7Message;
using HL7Messages;
using HL7MessageServer.Classes;
using HL7MessageServer.ErrorHandler;
using HL7MessageServer.Model;
using NHapi.Base.Parser;
using NHapi.Base.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
                ddlselect("ADT");
            }
        }

        protected void btnShow_Click(object sender, EventArgs e)
        {
            string folderPath = ddlfolderselect.SelectedValue;
            WCSHL7Entities wcs = new WCSHL7Entities();
            IList<Gridvalcheck> gridval = new List<Gridvalcheck>();
            DirectoryInfo info = new DirectoryInfo(folderPath);
            String[] allfiles = System.IO.Directory.GetFiles(folderPath, "*.*", System.IO.SearchOption.AllDirectories);
            foreach (string file in allfiles)
            {
                string message = File.ReadAllText(file);
                var parser = new PipeParser();
                var messageParsed = parser.Parse(message);
                Terser tst = new Terser(messageParsed);
                var returntype = tst.Get("/.MSH-9");
                try
                {
                    if(Convert.ToString(ddlShoworUpdate.SelectedValue)=="0")
                    {

                    }
                    else
                    {
                        if(returntype==ddlreturntype.SelectedValue)
                        {
                            HLMessageToDB hl7 = new HLMessageToDB();

                            hl7.HL7MessageToDB(message);
                        }
                       
                    }
                   


                    Gridvalcheck gvd = new Gridvalcheck();
                   

                    string externalPId = tst.Get("/.PID-3");
                    string mrnumber = txtMRnumber.Text;


                    if (externalPId == mrnumber || mrnumber != null || mrnumber != "")
                    {

                        gvd.filename = file;
                        gvd.admissionExtId = externalPId;
                        if (returntype == "ORM")
                        {
                            gvd.OrderID = tst.Get("/.ORC-2");
                            gvd.DBORDERSTATUS = AdmissionStatusId(tst.Get("/.PV1-41"));
                            string orderNumber = tst.Get("/.ORC-2");
                            int orderId = wcs.Order_tbl.Where(c => c.orderNumber == orderNumber).Select(d => d.orderId).FirstOrDefault();
                            gvd.OrderExistsInDB = Convert.ToString(orderId);
                        }
                        else
                        {
                            gvd.admissionExtId = tst.Get("/.PID-18");
                        }
                        gvd.filetime = File.GetLastWriteTime(file);
                        gvd.MessageType = returntype;
                       
                       
                        gridval.Add(gvd);

                    }
                    else
                    {

                        gvd.filename = file;
                        gvd.admissionExtId = externalPId;
                        if (returntype == "ORM")
                        {
                            gvd.OrderID = tst.Get("/.ORC-2");
                            gvd.DBORDERSTATUS = AdmissionStatusId(tst.Get("/.PV1-41"));

                            string orderNumber = tst.Get("/.ORC-2");
                            int orderId = wcs.Order_tbl.Where(c => c.orderNumber == orderNumber).Select(d => d.orderId).FirstOrDefault();
                            gvd.OrderExistsInDB = Convert.ToString(orderId);
                        }
                        else
                        {
                            gvd.admissionExtId = tst.Get("/.PID-18");
                        }
                        gvd.filetime = File.GetLastWriteTime(file);
                        gvd.MessageType = returntype;
                       
                        
                        gridval.Add(gvd);

                    }

                    
                   

                }
                catch (Exception ex)
                {
                    Gridvalcheck gvd = new Gridvalcheck();
                    gvd.filetime = File.GetLastWriteTime(file);
                    gvd.MessageType = returntype;
                    gvd.filename = file;
                    gvd.FileOrderStatus = ex.Message;
                    gridval.Add(gvd);
                }
                GridView1.DataSource = gridval;
                GridView1.DataBind();
            }

        }
        protected void ddlselect(string value)
        {
            string querystring = ConfigurationManager.AppSettings["MessageFolder"]+value; 
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
                      
                       
                        string externalPId = tst.Get("/PID-3");
                        gvd.filename = file;
                        gvd.admissionExtId = externalPId;
                        if (returntype == "ORM")
                        {
                            gvd.OrderID = tst.Get("/ORC-2");
                           


                        }
                        else
                        {
                            gvd.admissionExtId = tst.Get("/PID-18");
                        }
                        gvd.filetime = File.GetLastWriteTime(file);
                       
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
                    string adm1 = "";
                    string adm2 = "";
                    int admissiontypeid = AdmissionType.AdmissionTypeId(adm1,adm2);
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

        protected void btnUpdateRoom_Click(object sender, EventArgs e)
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
                    if (returntype == "ORM")
                    {
                        string value = proceduretimeupdate(tst);
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

        protected void btnUpdateAllergens_Click(object sender, EventArgs e)
        {
            string folderPath = ddlfolderselect.SelectedValue;
            WCSHL7Entities wcs = new WCSHL7Entities();
            IList<CCI> gridval = new List<CCI>();
            DirectoryInfo info = new DirectoryInfo(folderPath);
            String[] allfiles = System.IO.Directory.GetFiles(folderPath, "*.*", System.IO.SearchOption.AllDirectories);
            foreach (string file in allfiles)
            {
                string message = File.ReadAllText(file);
                var parser = new PipeParser();
                var messageParsed = parser.Parse(message);
                Terser tst = new Terser(messageParsed);
                CCI pd = new CCI();
                string regexOBX = @"\bOBX\b";
                string regexROL = @"\bROL\b";
                int OBXRep = Regex.Matches(message, regexOBX).Count;
                int ROLRep = Regex.Matches(message, regexROL).Count;
                string extPID = tst.Get("/.PID-3");
                try
                {

                    
                    string extpid = "";
                    var messageevent = tst.Get("/.MSH-9-2");
                    int patientIdCheck = 0;

                    extPID = tst.Get("/.PID-3");
                    patientIdCheck = wcs.Patient_tbl.Where(p => p.externalId == extPID).Select(pa => pa.patientId).FirstOrDefault();
                    if(patientIdCheck>0)
                    {
                        Patient_tbl pd1 = wcs.Patient_tbl.First(p => p.patientId == patientIdCheck);
                        ////Needs to be checked///
                        pd1.isAssistanceRequired = AllergenDetails.HasAllergy(tst, OBXRep, "Assistance"); 
                        pd1.isFallRisk = AllergenDetails.HasAllergy(tst, OBXRep, "FallRisk"); 
                        pd1.isMrsaPositive = AllergenDetails.HasAllergy(tst, OBXRep, "MRSA");
                        pd1.assistanceDescription = "";
                        pd1.hasLatexAllergy = AllergenDetails.HasAllergy(tst, OBXRep, "Latex");
                        try
                        {
                            wcs.SaveChanges();
                           
                        }
                        catch (DbEntityValidationException ex)
                        {
                            foreach (DbEntityValidationResult entityValidationError in ex.EntityValidationErrors)
                            {
                                string message1 = "Entity of type " + entityValidationError.Entry.Entity.GetType().Name + " in state " + (object)entityValidationError.Entry.State + "has the following validation errors:";
                                foreach (DbValidationError validationError in (IEnumerable<DbValidationError>)entityValidationError.ValidationErrors)
                                    HL7messageToFile.Exceptionhandler(message1, validationError.ErrorMessage);
                            }
                        }
                        pd.Assistance = regex("Assistance", message).ToString();
                        pd.fallsrisk = regex("FallRisk", message).ToString(); ;
                        pd.MRSA = regex("MRSA", message).ToString(); ;
                        pd.patientnumber = extPID;
                        pd.LatexAllergy = regex("Latex", message).ToString(); ;
                        gridval.Add(pd);
                    }
                    
                }
                catch (Exception ex)
                {
                    pd.patientnumber = extPID;
                    pd.error = ex.Message;
                    gridval.Add(pd);
                }

            }
            GridView1.DataSource = gridval;
            GridView1.DataBind();
        }

        protected void ddlreturntype_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlselect(ddlreturntype.SelectedValue);
        }

        protected void updateLocation_Click(object sender, EventArgs e)
        {
            string folderPath = ddlfolderselect.SelectedValue;
            WCSHL7Entities wcs = new WCSHL7Entities();
            IList<ErrorMessage> gridval = new List<ErrorMessage>();
            DirectoryInfo info = new DirectoryInfo(folderPath);
            String[] allfiles = System.IO.Directory.GetFiles(folderPath, "*.*", System.IO.SearchOption.AllDirectories);
            foreach (string file in allfiles)
            {
                string message = File.ReadAllText(file);
                var parser = new PipeParser();
                var messageParsed = parser.Parse(message);
                Terser tst = new Terser(messageParsed);
                Gridvalcheck gvd = new Gridvalcheck();
               
               
              
                
                try
                {
                    string orderNumber = tst.Get("/.ORC-2");
                    string procedure = tst.Get("/.OBR-4-2");
                    ErrorMessage err = new ErrorMessage();
                    err.Innermessage = procedure;
                   
                    err.Ordernumber = orderNumber;
                   
                    
                    err.Messagetype = "ORMAdmin";
                    err.ErrorDatetime = DateTime.Now.ToShortDateString();
                    err.MRCode = orderNumber;
                    err.AdmissionNumber = Convert.ToString(wcs.Procedures.Where(c => c.code == procedure).Select(d => d.ProcedureCategory_procedureCategoryId).FirstOrDefault());
                    gridval.Add(err);
                    //    int procedureId = wcs.Procedures.Where(s => s.code == procedure).Select(p => p.procedureId).FirstOrDefault();
                    //    string Department_location = tst.Get("/.OBR-18");
                    //    Department_location = ReturnLocation.location(Department_location, procedureId);
                    //    int departmentLocationId = Convert.ToInt32(wcs.Locations.Where(l => l.name == Department_location || l.code == Department_location).Select(locId => locId.locationId).FirstOrDefault());
                    //    string admextid = tst.Get("/.PID-18");
                    //    int admissionId = Convert.ToInt32(wcs.Admission_tbl.Where(adm => adm.externalId == admextid).Select(ad1 => ad1.admissionId).FirstOrDefault());
                    //    int ordercheck = Convert.ToInt32(wcs.Order_tbl.Where(c => c.orderNumber == orderNumber && (c.admissionId == admissionId)).FirstOrDefault());
                    //    if (ordercheck > 0)
                    //    {
                    //        Order_tbl ad = wcs.Order_tbl.First(c => c.orderNumber == orderNumber && (c.admissionId == admissionId));
                    //        ad.Department_locationId = departmentLocationId;
                    //        try
                    //        {
                    //            wcs.Order_tbl.Attach(ad);
                    //            wcs.Entry(ad).State = EntityState.Modified;
                    //            wcs.SaveChanges();

                    //        }
                    //        catch (DbEntityValidationException ex)
                    //        {

                    //            foreach (var eve in ex.EntityValidationErrors)
                    //            {
                    //                var exception = "Entity of type " + eve.Entry.Entity.GetType().Name + " in state " + eve.Entry.State + "has the following validation errors:";
                    //                foreach (var ve in eve.ValidationErrors)
                    //                {
                    //                    ErrorMessage err = new ErrorMessage();
                    //                    err.Innermessage = message;
                    //                    err.StackTrace = ve.ErrorMessage;
                    //                    err.Ordernumber = orderNumber;
                    //                    err.AdmissionNumber = admextid;
                    //                    err.EntityError = exception;
                    //                    err.FileName = file;
                    //                    err.Messagetype = "ORMAdmin";
                    //                    err.ErrorDatetime = DateTime.Now.ToShortDateString();
                    //                    err.MRCode = orderNumber;
                    //                    XMLCreator.xmlwriter(err);
                    //                    gridval.Add(err);
                    //                }
                    //            }

                    //        }
                    //    }
                }
                catch (Exception ex)
                {
                    //ErrorMessage err = new ErrorMessage();
                    //err.Innermessage = message;
                    //err.StackTrace = ex.Message;
                    //err.Ordernumber = "NA";
                    //err.AdmissionNumber = "NA";
                    //err.EntityError = ex.StackTrace;
                    //err.FileName = file;
                    //err.Messagetype = "ORMAdmin";
                    //err.ErrorDatetime = DateTime.Now.ToShortDateString();
                    //err.MRCode = "Unknown";
                    //XMLCreator.xmlwriter(err);
                    //gridval.Add(err);
                }

            }
            GridView1.DataSource = gridval;
            GridView1.DataBind();
        }
        public bool regex(string query,string message)
        {
            int count1 = Regex.Matches(message, query).Count;
               if(count1>0)
            {
                return true;
            }
               else
            {
                return false;
            }
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
public class CCI
{
    public string patientnumber { get; set; }
    public string fallsrisk { get; set; }
    public string MRSA { get; set; }
    public string Assistance { get; set; }
    public string LatexAllergy { get; set; }

    public string error { get; set; }

}

