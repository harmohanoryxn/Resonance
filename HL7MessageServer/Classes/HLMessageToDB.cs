using H7Message;
using NHapi.Base.Model;
using NHapi.Base.Parser;
using NHapi.Base.Util;
using NHapi.Model.V23.Message;
using NHapi.Model.V24.Segment;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Data.Entity.Migrations;
using HL7MessageServer.Model;
using System.Data.Entity;
using HL7MessageServer.Helpers;
using HL7Messages;
using System.Reflection;
using HL7MessageServer.Classes;

namespace H7Message
{
    public class HLMessageToDB
    {

        public void HL7MessageToDB(string message,string filename)
        {
            var parser = new PipeParser();
            var messageParsed = parser.Parse(message);
            Terser tst = new Terser(messageParsed);
            var MessageType = tst.Get("/MSH-9");

            string regexOBX = @"\bOBX\b";
            string regexROL = @"\bROL\b";
            int OBXRep = Regex.Matches(message, regexOBX).Count;
            int ROLRep = Regex.Matches(message, regexROL).Count;


            switch (MessageType)
            {
                case "ORM":
                    ormMessage(tst, message, OBXRep,filename);
                    break;
                case "ADT":
                    adtMessage(tst, OBXRep, ROLRep,filename);
                    break;
                    //case "ORU":
                    //    oruMessage(tst);
                    //    break;



            }


        }
        protected string ormMessage(Terser tst, string message, int obxrep,string filename)
        {
            string result = "";
            WCSHL7Entities wcs = new WCSHL7Entities();
            string patientlocationCheck = tst.Get("/PV1-3");
            String deploccheck= tst.Get("/OBR-18");
            if (patientlocationCheck == "LIMERICK" || deploccheck == "LIMERICK")
            {

            }
            else
            {


                int admissiontypeid = AdmissionType.AdmissionTypeId(tst);
                string pid = tst.Get("/PID-3");
                string procedure = tst.Get("/OBR-4-2");
                var procedureIdCheck = wcs.Procedures.Where(s => s.code == procedure).Select(p => p.procedureId).FirstOrDefault().ToString();
                if (procedureIdCheck == "0" || procedureIdCheck == null)
                {
                    Procedure pc = new Procedure();
                    pc.externalId = procedure + "1";
                    pc.code = procedure;
                    pc.description = tst.Get("OBR-4-3");
                    pc.externalSourceId = 1;
                    string extID = tst.Get("/OBR-4");
                    var procedurecategory = wcs.ProcedureCategories.Where(procat => procat.externalId == extID).Select(r => r.procedureCategoryId).FirstOrDefault().ToString();
                    if (procedurecategory == "0")
                    {
                        ProcedureCategory pCat = new ProcedureCategory();
                        pCat.externalSourceId = 1;
                        pCat.externalId = extID;
                        pCat.includeInMerge = true;
                        pCat.description = "";
                        wcs.ProcedureCategories.AddOrUpdate(pCat);
                        wcs.SaveChanges();
                        Insertionhelper.insertdata("ProcedureCategory", 0, "Procedure category Insertion");
                    }
                    var procedurecategoryID = wcs.ProcedureCategories.Where(procat => procat.externalId == extID).Select(r => r.procedureCategoryId).FirstOrDefault().ToString();
                    pc.ProcedureCategory_procedureCategoryId = Convert.ToInt32(procedurecategoryID);
                    wcs.Procedures.AddOrUpdate(pc);
                    wcs.SaveChanges();
                    Insertionhelper.insertdata("Procedure", 0, "Procedure  Insertion");
                }
                int procedureId = wcs.Procedures.Where(s => s.code == procedure).Select(p => p.procedureId).FirstOrDefault();


                string admitdatetime = DateTime.Now.ToShortDateString();
                string extSource = tst.Get("/OBR-16");

                string extId = tst.Get("/OBR-18-2");
                string orderNumber = tst.Get("/ORC-2");
                string clinicalIndicator = tst.Get("OBR-3-2");



                string ProcedureTimeduration = tst.Get("/OBR-6");
                string ProcedureName = tst.Get("/OBR-4");
                string status = tst.Get("/ORC-5");
                string Department_location = tst.Get("/OBR-18");
                Department_location = ReturnLocation.location(Department_location);
                switch (status)
                {
                    case "L":
                        status = "InProgress";
                        break;
                    case "I":
                        status = "InProgress";
                        break;
                    case "T":
                        if (Department_location == "MRI" || Department_location == "X-Ray" || Department_location == "Fluoroscopy" || Department_location == "Nuclear Medicine" || Department_location == "CT Scan" || Department_location == "Ultrasound")
                        {
                            status = "Completed";
                        }
                        else
                        {
                            status = "InProgress";
                        }
                       
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

                int orderstatusId = Convert.ToInt32(wcs.OrderStatus.Where(os => os.status == status).Select(osId => osId.orderStatusId).FirstOrDefault());
                int departmentLocCheck = Convert.ToInt32(wcs.Locations.Where(l => l.name == Department_location || l.code == Department_location).Select(locId => locId.locationId).FirstOrDefault());
                if (departmentLocCheck <= 0)
                {
                    Location loc = new Location();
                    loc.name = Department_location;
                    loc.code = Department_location;
                    loc.isEmergency = false;
                    loc.includeInMerge = true;
                    wcs.Locations.AddOrUpdate(loc);
                    wcs.SaveChanges();
                    Insertionhelper.insertdata("Location", 0, "Location  Insertion");
                }
                int departmentLocationId = Convert.ToInt32(wcs.Locations.Where(l => l.name == Department_location || l.code == Department_location).Select(locId => locId.locationId).FirstOrDefault());
                string admextid = tst.Get("/PID-18");
                int admissionId = Convert.ToInt32(wcs.Admission_tbl.Where(adm => adm.externalId == admextid).Select(ad => ad.admissionId).FirstOrDefault());
                string OrderingDocFirstname = tst.Get("/OBR-16-3");
                string OrderingDoclastname = tst.Get("/OBR-16-2");
                string OrderingDocMiddle = tst.Get("/OBR-16-4");
                string OrderingDocPrefx = tst.Get("/OBR-16-5");
                string orderingDocMnemonic = tst.Get("/OBR-16");
                int doctorNameCheck = Convert.ToInt32(wcs.Doctors.Where(d => d.externalId == orderingDocMnemonic).Select(doc => doc.doctorId).FirstOrDefault());

                if (doctorNameCheck <= 0)
                {
                    Doctor doc = new Doctor();
                    doc.externalSourceId = 3;
                    doc.externalId = orderingDocMnemonic;
                    doc.name = OrderingDocPrefx + " " + OrderingDocFirstname + " " + OrderingDocMiddle + " " + OrderingDoclastname;
                    wcs.Doctors.AddOrUpdate(doc);
                    wcs.SaveChanges();
                    Insertionhelper.insertdata("Doctor", 0, "Doctor  Insertion");
                }
                int orderDoctorId = Convert.ToInt32(wcs.Doctors.Where(d => d.externalId == orderingDocMnemonic).Select(doc => doc.doctorId).FirstOrDefault());
                Order_tbl ordertbl = new Order_tbl();
                ordertbl.externalSourceId = 2;
                ordertbl.externalId = orderNumber;
                ordertbl.orderNumber = orderNumber;

                string proceduretimecheck = tst.Get("/OBR-27-4");
                var ProcedureTime = "";
                if (proceduretimecheck != null || proceduretimecheck != "")
                {
                    if (proceduretimecheck.Length > 8)
                    {
                        ProcedureTime = DateTime.ParseExact(proceduretimecheck, "yyyyMMddHHmm", null).ToString("yyyy-MM-dd HH:mm");
                    }
                    else if (proceduretimecheck.Length == 8)
                    {
                        ProcedureTime = DateTime.ParseExact(proceduretimecheck, "yyyyMMdd", null).ToString("yyyy-MM-dd HH:mm");
                    }



                }
                if (obxrep > 0)
                {
                    for (int i = 0; i < obxrep; i++)
                    {
                        string value = tst.Get("/OBX(" + i + ")-3-2");
                        if (value.Contains("Clinical Indication") || value.Contains("Clinical indication") || value.Contains("Clinical Indicator"))
                        {
                            clinicalIndicator = tst.Get("/OBX(" + i + ")-5");
                            clinicalIndicator += " " + tst.Get("/OBX(" + i + ")-5-2");
                        }
                    }
                }
                int admissiontype = AdmissionType.AdmissionTypeId(tst);
                int patitentlocationid = PatientLocation.PatientLocationId(tst);
                string assignedpatientlocation = tst.Get("/PV1-3");
                if (assignedpatientlocation == "ER" || assignedpatientlocation == "A&E" || assignedpatientlocation == "AE")
                {
                    string resultadm = admissiontypeupdate(admissionId, 1, patitentlocationid, admissiontype);

                    TimeSpan ts = new TimeSpan(00, 00, 00);
                    DateTime dt = Convert.ToDateTime(Convert.ToDateTime(ProcedureTime).Date + ts);
                    ordertbl.procedureTime = dt;

                }

                else
                {
                    string resultadm = admissiontypeupdate(admissionId, admissiontype, patitentlocationid, 0);
                    if (admissiontypeid == 1)
                    {

                        TimeSpan ts = new TimeSpan(00, 00, 00);
                        DateTime dt = Convert.ToDateTime(Convert.ToDateTime(ProcedureTime).Date + ts);
                        ordertbl.procedureTime = dt;
                    }
                    else
                    {
                        ordertbl.procedureTime = Convert.ToDateTime(ProcedureTime);
                    }
                }

                ordertbl.orderStatusId = orderstatusId;
                ordertbl.admissionId = admissionId;
                ordertbl.Procedure_procedureId = procedureId;
                ordertbl.clinicalIndicator = clinicalIndicator;
                ordertbl.Department_locationId = departmentLocationId;
                ordertbl.OrderingDoctor_doctorId = orderDoctorId;
                ordertbl.isHidden = false;
                ordertbl.acknowledged = false;
                ////////Need to check these and create queries//////
                ordertbl.requiresFootwear = false;
                ordertbl.requiresMedicalRecords = false;
                ordertbl.requiresSupervision = false;


                try
                {

                    int orderId = wcs.Order_tbl.Where(c => c.orderNumber == orderNumber).Select(d => d.orderId).FirstOrDefault();
                    if (orderId > 0)
                    {
                        Order_tbl ad = wcs.Order_tbl.First(c => c.orderNumber == orderNumber && (c.admissionId == admissionId));
                        if (admissiontypeid == 1)
                        {
                            var procedureupdatetimecheck = wcs.Updates.Where(c => c.Order_orderId == ad.orderId && c.type == "Procdure Time Updated").FirstOrDefault();
                            if(procedureupdatetimecheck !=null)
                            {
                                DateTime datecreated = Convert.ToDateTime(procedureupdatetimecheck.dateCreated);
                                TimeSpan ts = TimeSpan.Parse(procedureupdatetimecheck.value);
                                DateTime updatedtime = Convert.ToDateTime(datecreated.Date +ts );
                                DateTime hl7PRoceduredatetime=Convert.ToDateTime(ProcedureTime);
                                if(updatedtime<hl7PRoceduredatetime)
                                {
                                    ad.procedureTime= Convert.ToDateTime(ProcedureTime);
                                }
                            }
                           
                        }
                        else
                        {
                            ad.procedureTime = Convert.ToDateTime(ProcedureTime);
                        }
                        ad.orderStatusId = orderstatusId;
                        ad.Procedure_procedureId = procedureId;
                        
                        ad.clinicalIndicator = clinicalIndicator;
                        ad.Department_locationId = departmentLocationId;
                        ad.OrderingDoctor_doctorId = orderDoctorId;
                        wcs.Order_tbl.Attach(ad);
                        wcs.Entry(ad).State = EntityState.Modified;
                        wcs.SaveChanges();
                        Insertionhelper.insertdata(orderNumber, orderId, "Order Updated");

                    }
                    else if (admissionId == 0)
                    {

                    }
                    else
                    {

                        wcs.Order_tbl.Add(ordertbl);
                        wcs.SaveChanges();
                        int orderIdnw = wcs.Order_tbl.Where(c => c.orderNumber == orderNumber).Select(d => d.orderId).FirstOrDefault();
                        Insertionhelper.insertdata(orderNumber, orderIdnw, "Order Imported");
                    }

                }

                catch (DbEntityValidationException e)
                {

                    foreach (var eve in e.EntityValidationErrors)
                    {
                        var exception = "Entity of type " + eve.Entry.Entity.GetType().Name + " in state " + eve.Entry.State + "has the following validation errors:";
                        foreach (var ve in eve.ValidationErrors)
                        {
                            HL7messageToFile.Exceptionhandler(exception+"File Name: "+filename, ve.ErrorMessage);

                        }
                    }
                    throw;
                }



            }

            return result;
        }

        protected string adtMessage(Terser tst, int obxrep, int rolrep,string filename)
        {
            string result = "";
            try
            {
                WCSHL7Entities wcs = new WCSHL7Entities();
                Admission_tbl admissiontbl = new Admission_tbl();
                bool rol = false;
                string extPID = tst.Get("/PID-3");
                /// initializing parameters///
                ///Calling patient info class for getting patient ID/////
                int patientId = PatientInfo.PatientinfoReturn(tst, obxrep); ;
                int patientlocationId = 0;
                int admissionTypeId = 0;
                string externalPId = tst.Get("/PID-3");
                string admexternalId = tst.Get("/PID-18");
                string messageEventTrigger = tst.Get("/MSH-9-2");
                int admissioncheck = wcs.Admission_tbl.Where(c => c.externalId == admexternalId).Select(d => d.admissionId).FirstOrDefault();
                if (admissioncheck > 0)
                {
                    Admission_tbl ad = wcs.Admission_tbl.First(i => i.externalId == admexternalId);
                    ////Getting patient location id//////
                    patientlocationId = PatientLocation.PatientLocationId(tst);
                    //////Getting patient admission type id
                    admissionTypeId = AdmissionType.AdmissionTypeId(tst);
                    string assignedpatientlocation = tst.Get("/PV1-3");
                    if (assignedpatientlocation == "ER" || assignedpatientlocation == "A&E" || assignedpatientlocation == "AE")
                    {
                    }
                    else
                    {
                        if (admissionTypeId > 0)
                        {
                            ad.AdmissionType_admissionTypeId = admissionTypeId;
                        }
                    }
                    rol = true;

                    string Patientadmitdatetime = DateTime.ParseExact(tst.Get("PV1-44").ToString(), "yyyyMMddHHmm", null).ToString("yyyy-MM-dd HH:mm");
                    ad.externalSourceId = ExtSourceId(tst.Get("/MSH-4"));
                    ad.patientId = patientId;
                    ad.externalId = admexternalId;
                    ad.AdmissionStatus_admissionStatusId = AdmissionStatusId(tst.Get("/PV1-41"));
                    if (patientlocationId > 0)
                    {
                        ad.Location_locationId = patientlocationId;
                    }
                    else
                    {
                        ad.Location_locationId = ad.Location_locationId;
                    }
                    ad.admitDateTime = Convert.ToDateTime(Patientadmitdatetime);
                    if (rol == false)
                    {

                    }
                    else
                    {
                        for (int i = 0; i < rolrep; i++)
                        {
                            string roleProvider = tst.Get("/ROL(" + i + ")-3");
                            string rolepersonMnemonic = tst.Get("/ROL(" + i + ")-4");
                            string rolepersonFirstName = tst.Get("/ROL(" + i + ")-4-3");
                            string rolepersonMiddleName = tst.Get("/ROL(" + i + ")-4-4");
                            string rolepersonLastName = tst.Get("/ROL(" + i + ")-4-2");
                            string rolepersonPrefix = tst.Get("/ROL(" + i + ")-4-5");
                            if (roleProvider != null || roleProvider == "")
                            {
                                int DocId = DoctorId(rolepersonMnemonic, rolepersonPrefix, rolepersonFirstName, rolepersonLastName, rolepersonMiddleName);
                                switch (roleProvider)
                                {
                                    case "AD":
                                        ad.AdmittingDoctor_doctorId = DocId;
                                        break;
                                    case "AT":
                                        ad.AttendingDoctor_doctorId = DocId;
                                        break;
                                    case "PP":
                                        ad.PrimaryCareDoctor_doctorId = DocId;
                                        break;
                                }
                            }
                        }
                    }


                    try
                    {
                        wcs.SaveChanges();
                        Insertionhelper.insertdata(admexternalId, admissioncheck, "Admission Updated");
                    }
                    catch (DbEntityValidationException e)
                    {

                        foreach (var eve in e.EntityValidationErrors)
                        {
                            var exception = "Entity of type " + eve.Entry.Entity.GetType().Name + " in state " + eve.Entry.State + "has the following validation errors:";
                            foreach (var ve in eve.ValidationErrors)
                            {
                                HL7messageToFile.Exceptionhandler(exception, ve.ErrorMessage + "File name:" + filename);

                            }
                        }

                    }
                }
                else
                {
                    ////Getting patient location id//////
                    patientlocationId = PatientLocation.PatientLocationId(tst);
                    //////Getting patient admission type id

                    admissionTypeId = AdmissionType.AdmissionTypeId(tst);
                    if (admissionTypeId > 0)
                    {
                        admissiontbl.AdmissionType_admissionTypeId = admissionTypeId;
                    }
                    rol = true;

                    string Patientadmitdatetime = DateTime.ParseExact(tst.Get("PV1-44").ToString(), "yyyyMMddHHmm", null).ToString("yyyy-MM-dd HH:mm");
                    admissiontbl.externalSourceId = ExtSourceId(tst.Get("/MSH-4"));
                    admissiontbl.patientId = patientId;
                    admissiontbl.externalId = admexternalId;
                    admissiontbl.AdmissionStatus_admissionStatusId = AdmissionStatusId(tst.Get("/PV1-41"));
                    if (patientlocationId > 0)
                    {
                        admissiontbl.Location_locationId = patientlocationId;
                    }
                    admissiontbl.admitDateTime = Convert.ToDateTime(Patientadmitdatetime);


                    /////iterating through ROL for doc information/////
                    if (rol == false)
                    {

                    }
                    else
                    {
                        for (int i = 0; i < rolrep; i++)
                        {
                            string roleProvider = tst.Get("/ROL(" + i + ")-3");
                            string rolepersonMnemonic = tst.Get("/ROL(" + i + ")-4");
                            string rolepersonFirstName = tst.Get("/ROL(" + i + ")-4-3");
                            string rolepersonMiddleName = tst.Get("/ROL(" + i + ")-4-4");
                            string rolepersonLastName = tst.Get("/ROL(" + i + ")-4-2");
                            string rolepersonPrefix = tst.Get("/ROL(" + i + ")-4-5");
                            if (roleProvider != null || roleProvider == "")
                            {
                                int DocId = DoctorId(rolepersonMnemonic, rolepersonPrefix, rolepersonFirstName, rolepersonLastName, rolepersonMiddleName);
                                switch (roleProvider)
                                {
                                    case "AD":
                                        admissiontbl.AdmittingDoctor_doctorId = DocId;
                                        break;
                                    case "AT":
                                        admissiontbl.AttendingDoctor_doctorId = DocId;
                                        break;
                                    case "PP":
                                        admissiontbl.PrimaryCareDoctor_doctorId = DocId;
                                        break;
                                }
                            }
                        }
                    }
                    wcs.Admission_tbl.Add(admissiontbl);
                    try
                    {
                        wcs.SaveChanges();
                        int externalIdcheck1 = wcs.Admission_tbl.Where(c => c.externalId == admexternalId).Select(ad => ad.admissionId).FirstOrDefault();
                        Insertionhelper.insertdata(admexternalId, externalIdcheck1, "Admission Imported");
                    }
                    catch (DbEntityValidationException e)
                    {

                        foreach (var eve in e.EntityValidationErrors)
                        {
                            var exception = "Entity of type " + eve.Entry.Entity.GetType().Name + " in state " + eve.Entry.State + "has the following validation errors:";
                            foreach (var ve in eve.ValidationErrors)
                            {
                                HL7messageToFile.Exceptionhandler(exception, ve.ErrorMessage + " File name:" + filename);

                            }
                        }

                    }
                }


            }
            catch (Exception ex)
            {
                result = ex.Message;
                string innerexception = Convert.ToString(ex.InnerException);
                HL7messageToFile.Exceptionhandler(ex.Message + " File name:" + filename + "Internal error" + innerexception, Convert.ToString(ex.StackTrace));
            }
            return result;
        }
     
        private int DoctorId(string docCode, string prefix, string firstname, string lastname, string middlename)
        {
            int DocId;
            WCSHL7Entities wcs = new WCSHL7Entities();
            DocId = Convert.ToInt32(wcs.Doctors.Where(d => d.externalId == docCode).Select(doc => doc.doctorId).FirstOrDefault());
            if (DocId <= 0)
            {
                Doctor doc = new Doctor();
                doc.externalSourceId = 3;
                doc.externalId = docCode;
                doc.name = prefix + " " + firstname + " " + middlename + " " + lastname;
                wcs.Doctors.AddOrUpdate(doc);
                wcs.SaveChanges();
            }
            DocId = Convert.ToInt32(wcs.Doctors.Where(d => d.externalId == docCode).Select(doc => doc.doctorId).FirstOrDefault());
            return DocId;
        }
        private int ExtSourceId(string ExtSource)
        {
            int ExtId;
            WCSHL7Entities wcs = new WCSHL7Entities();
            ExtId = Convert.ToInt32(wcs.ExternalSources.Where(e => e.source == ExtSource).Select(ext => ext.externalSourceId).FirstOrDefault());
            if (ExtId <= 0)
            {
                ExternalSource ext = new ExternalSource();
                ext.source = ExtSource;
                wcs.ExternalSources.AddOrUpdate(ext);
                wcs.SaveChanges();
            }
            ExtId = Convert.ToInt32(wcs.ExternalSources.Where(e => e.source == ExtSource).Select(ext => ext.externalSourceId).FirstOrDefault());
            return ExtId;
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
        private string admissiontypeupdate(int admissionid, int admtype, int patientlocationid,int historicaldata)
        {
            string result = "";
            WCSHL7Entities wcs = new WCSHL7Entities();
            Admission_tbl adm = wcs.Admission_tbl.First(c => c.admissionId == admissionid);

            int presentadmtype = adm.AdmissionType_admissionTypeId;
            if (admtype > 0)
            {
                adm.AdmissionType_admissionTypeId = admtype;
            }
            if (patientlocationid > 0)
            {
                adm.Location_locationId = patientlocationid;
            }
           
            try
            {
                wcs.Admission_tbl.Attach(adm);
                wcs.Entry(adm).State = EntityState.Modified;
                wcs.SaveChanges();

                Insertionhelper.insertdata(Convert.ToString(admissionid), admissionid, "Admission Updated");
            }
            catch (DbEntityValidationException e)
            {

                foreach (var eve in e.EntityValidationErrors)
                {
                    var exception = "Entity of type " + eve.Entry.Entity.GetType().Name + " in state " + eve.Entry.State + "has the following validation errors:";
                    foreach (var ve in eve.ValidationErrors)
                    {
                        HL7messageToFile.Exceptionhandler(exception +" File name", ve.ErrorMessage);

                    }
                }
                return "exception";
            }

            return "success";

        }


    }
}
