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

namespace H7Message
{
    public class HLMessageToDB
    {
        
        public void HL7MessageToDB(string message)
        {
            var parser = new PipeParser();
            var messageParsed = parser.Parse(message);
            string regexOBX = @"\bOBX\b";
            string regexROL = @"\bROL\b";
            int OBXRep = Regex.Matches(message, regexOBX).Count;
            int ROLRep = Regex.Matches(message, regexROL).Count;
            Terser tst = new Terser(messageParsed);
            var MessageType = tst.Get("/MSH-9");
            switch(MessageType)
            {
                case "ORM":
                    ormMessage(tst);
                    break;
                case "ADT":
                    adtMessage(tst,OBXRep,ROLRep);
                    break;



            }
            

        }
        protected string ormMessage(Terser tst)
        {
            string result = "";
            WCSEntities wcs = new WCSEntities();
                      
                   
                    string pid = tst.Get("/PID-2");
                    string procedure = tst.Get("/OBR-4-2");
                    var procedureIdCheck = wcs.Procedures.Where(s => s.code ==procedure).Select(p=>p.procedureId).FirstOrDefault().ToString();
                    if(procedureIdCheck == "0" || procedureIdCheck == null)
                    {
                        Procedure pc = new Procedure();
                        pc.externalId = procedure+"1";
                        pc.code = procedure;
                        pc.description = tst.Get("OBR-4-3");
                        pc.externalSourceId = 1;
                        string extID = tst.Get("/OBR-4");
                        var procedurecategory = wcs.ProcedureCategories.Where(procat => procat.externalId == extID).Select(r => r.procedureCategoryId).FirstOrDefault().ToString();
                        if(procedurecategory=="0")
                        {
                            ProcedureCategory pCat = new ProcedureCategory();
                            pCat.externalSourceId = 1;
                            pCat.externalId = extID;
                            pCat.includeInMerge = true;
                            pCat.description = "";
                            wcs.ProcedureCategories.Add(pCat);
                            wcs.SaveChanges();

                        }
                        var procedurecategoryID = wcs.ProcedureCategories.Where(procat => procat.externalId == extID).Select(r => r.procedureCategoryId).FirstOrDefault().ToString();
                        pc.ProcedureCategory_procedureCategoryId = Convert.ToInt32(procedurecategoryID);
                        wcs.Procedures.Add(pc);
                        wcs.SaveChanges();
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
            switch (status)
            {
                case "L":
                    status = "Logged";
                    break;
                case "I":
                    status = "In Process";
                    break;
                case "T":
                    status = "Taken";
                    break;
                case "C":
                    status = "Completed";
                    break;
                case "R":
                    status = "Resulted";
                    break;
                case "X":
                    status = "Cancelled";
                    break;
            }
            int orderstatusIdCheck = Convert.ToInt32(wcs.OrderStatus.Where(os=>os.status== status).Select(osId=>osId.orderStatusId).FirstOrDefault());
                    if(orderstatusIdCheck == 0)
                    {
                OrderStatu orderstatus = new OrderStatu();
                orderstatus.status=status;
                wcs.OrderStatus.Add(orderstatus);
                wcs.SaveChanges();
                
                    }
            int orderstatusId = Convert.ToInt32(wcs.OrderStatus.Where(os => os.status == status).Select(osId => osId.orderStatusId).FirstOrDefault());
            string Department_location = tst.Get("/ORC-13");
                    int departmentLocCheck = Convert.ToInt32(wcs.Locations.Where(l => l.name == Department_location).Select(locId => locId.locationId).FirstOrDefault());
                    if(departmentLocCheck<=0)
                    {
                        Location loc = new Location();
                        loc.name = Department_location;
                        loc.code = Department_location;
                        loc.isEmergency = false;
                        loc.includeInMerge = true;
                        wcs.Locations.Add(loc);
                        wcs.SaveChanges();
                    }
                    int departmentLocationId= Convert.ToInt32(wcs.Locations.Where(l => l.name == Department_location).Select(locId => locId.locationId).FirstOrDefault());
                    string extid = tst.Get("/PID-2");
                    int admissionId =Convert.ToInt32(wcs.Admission_tbl.Where(adm => adm.patientId == wcs.Patient_tbl.Where(patien => patien.externalId ==pid ).Select(p => p.patientId).FirstOrDefault()).Select(ad=>ad.admissionId).FirstOrDefault());
                    string OrderingDocFirstname = tst.Get("/OBR-16-3");
                    string OrderingDoclastname = tst.Get("/OBR-16-2");
                    string OrderingDocMiddle = tst.Get("/OBR-16-4");
                    string OrderingDocPrefx = tst.Get("/OBR-16-5");
                    string orderingDocMnemonic = tst.Get("/OBR-16");
                    int doctorNameCheck = Convert.ToInt32(wcs.Doctors.Where(d=>d.externalId==orderingDocMnemonic).Select(doc=>doc.doctorId).FirstOrDefault());
                    
                    if (doctorNameCheck <= 0)
                    {
                        Doctor doc = new Doctor();
                        doc.externalSourceId = 3;
                        doc.externalId = orderingDocMnemonic;
                        doc.name = OrderingDocPrefx + " " + OrderingDocFirstname + " " + OrderingDocMiddle + " " + OrderingDoclastname;
                        wcs.Doctors.Add(doc);
                        wcs.SaveChanges();
                    }
                    int orderDoctorId = Convert.ToInt32(wcs.Doctors.Where(d => d.externalId == orderingDocMnemonic).Select(doc => doc.doctorId).FirstOrDefault());
                    Order_tbl ordertbl = new Order_tbl();
                    ordertbl.externalSourceId = 1;
                    ordertbl.externalId = extid;
                    ordertbl.orderNumber = orderNumber;
            string proceduretimecheck = tst.Get("/OBR-27-4");
            var ProcedureTime = "";
            if (proceduretimecheck != null || proceduretimecheck!="")
            {
                ProcedureTime = DateTime.ParseExact(proceduretimecheck, "yyyyMMddHHmm", null).ToString("yyyy-MM-dd HH:mm");
                ordertbl.procedureTime = Convert.ToDateTime(ProcedureTime);
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
                    wcs.Order_tbl.Add(ordertbl);
            try
            {
                wcs.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {

                foreach (var eve in e.EntityValidationErrors)
                {
                    var exception = "Entity of type " + eve.Entry.Entity.GetType().Name + " in state " + eve.Entry.State + "has the following validation errors:";
                    foreach (var ve in eve.ValidationErrors)
                    {
                        var errors = " Property: " + ve.PropertyName + ", Error: " + ve.ErrorMessage;
                    }
                }
                throw;
            }





            return result;
        }
        
        protected string adtMessage(Terser tst,int obxrep,int rolrep)
        {
            string result = "";
            try
            {
                WCSEntities wcs = new WCSEntities();
                Admission_tbl admissiontbl = new Admission_tbl();
               

               
                string admitdatetime = DateTime.Now.ToShortDateString();
                string extPID = tst.Get("/PID-2");


                int patientIdCheck = wcs.Patient_tbl.Where(p => p.externalId == extPID).Select(pa => pa.patientId).FirstOrDefault();
                if (patientIdCheck == 0)
                {
                    Patient_tbl patienttbl = new Patient_tbl();
                    patienttbl.externalSourceId = 1;
                    patienttbl.externalId = extPID;
                    patienttbl.givenName = tst.Get("/PID-5-2");
                    patienttbl.surname = tst.Get("/PID-5");
                    var dob = DateTime.ParseExact(tst.Get("/PID-7"), "yyyyMMdd", null).ToString("yyyy-MM-dd HH:mm");
                    patienttbl.dob = Convert.ToDateTime(dob);
                    patienttbl.sex = tst.Get("/PID-8");
                    ////Needs to be checked///
                    patienttbl.isAssistanceRequired = HasAllergy(tst, obxrep, "Assistance"); ;
                    patienttbl.isFallRisk = HasAllergy(tst, obxrep, "PCS.NDADM111"); ;
                    patienttbl.isMrsaPositive = HasAllergy(tst, obxrep, "PCS.NDADM054D"); ;
                    patienttbl.assistanceDescription = "";
                    patienttbl.hasLatexAllergy = HasAllergy(tst,obxrep,"ADM.Allergy");
                    wcs.Patient_tbl.Add(patienttbl);
                    wcs.SaveChanges();
                }
                int patientId = wcs.Patient_tbl.Where(p => p.externalId == extPID).Select(pa => pa.patientId).FirstOrDefault();
                string assignedpatientlocation = tst.Get("/PV1-3");
                string assignedpatientroom = tst.Get("/PV1-3-2");
                string assignedpatientBed = tst.Get("/PV1-3-3");
                int patientlocationcheck = Convert.ToInt32(wcs.Locations.Where(l => l.name == assignedpatientlocation).Select(locId => locId.locationId).FirstOrDefault());
                if (patientlocationcheck <= 0)
                {
                    Location loc = new Location();
                    loc.name = assignedpatientlocation;
                    loc.code = assignedpatientlocation;
                    loc.isEmergency = false;
                    loc.includeInMerge = true;
                    wcs.Locations.Add(loc);
                    wcs.SaveChanges();
                }

                int patientlocationId = Convert.ToInt32(wcs.Locations.Where(l => l.name == assignedpatientlocation).Select(locId => locId.locationId).FirstOrDefault());
                string admissiontype = tst.Get("/PV1-18");
                int admissionTypecheck = Convert.ToInt32(wcs.tbl_AdmissionType.Where(a => a.type == admissiontype).Select(ad => ad.admissionTypeId).FirstOrDefault());
                if (admissionTypecheck <= 0)
                {
                    tbl_AdmissionType admtype = new tbl_AdmissionType();
                    admtype.type = admissiontype;
                    wcs.tbl_AdmissionType.Add(admtype);
                    try
                    {
                        wcs.SaveChanges();
                    }
                    catch (DbEntityValidationException e)
                    {

                        foreach (var eve in e.EntityValidationErrors)
                        {
                            var exception = "Entity of type " + eve.Entry.Entity.GetType().Name + " in state " + eve.Entry.State + "has the following validation errors:";
                            foreach (var ve in eve.ValidationErrors)
                            {
                                var errors = " Property: " + ve.PropertyName + ", Error: " + ve.ErrorMessage;
                            }
                        }
                        throw;
                    }
                }
                int admissionTypeId = Convert.ToInt32(wcs.tbl_AdmissionType.Where(a => a.type == admissiontype).Select(ad => ad.admissionTypeId).FirstOrDefault());


                

                /////iterating through ROL for doc information/////
                for(int i=0;i<rolrep;i++)
                {
                    string roleProvider = tst.Get("/ROL("+i+")-3");
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
                
                admissiontbl.externalSourceId = ExtSourceId(tst.Get("/MSH-4"));
                admissiontbl.patientId = patientId;
                admissiontbl.externalId = tst.Get("/PID-3");

                admissiontbl.AdmissionStatus_admissionStatusId = AdmissionStatusId(tst.Get("/PV1-41"));
                admissiontbl.AdmissionType_admissionTypeId = admissionTypeId;

                admissiontbl.Location_locationId = patientlocationId;
                wcs.Admission_tbl.Add(admissiontbl);
                wcs.SaveChanges();
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
        private int DoctorId(string docCode,string prefix,string firstname,string lastname, string middlename)
        {
            int DocId;
            WCSEntities wcs = new WCSEntities();
            DocId = Convert.ToInt32(wcs.Doctors.Where(d => d.externalId == docCode).Select(doc => doc.doctorId).FirstOrDefault());
            if (DocId <= 0)
            {
                Doctor doc = new Doctor();
                doc.externalSourceId = 3;
                doc.externalId = docCode;
                doc.name = prefix + " " + firstname + " " + middlename + " " + lastname;
                wcs.Doctors.Add(doc);
                wcs.SaveChanges();
            }
            DocId = Convert.ToInt32(wcs.Doctors.Where(d => d.externalId == docCode).Select(doc => doc.doctorId).FirstOrDefault());
            return DocId;
        }
        private int ExtSourceId(string ExtSource)
        {
            int ExtId;
            WCSEntities wcs = new WCSEntities();
            ExtId = Convert.ToInt32(wcs.ExternalSources.Where(e => e.source == ExtSource).Select(ext => ext.externalSourceId).FirstOrDefault());
            if (ExtId <= 0)
            {
                ExternalSource ext = new ExternalSource();
                ext.source = ExtSource;
                wcs.ExternalSources.Add(ext);
                wcs.SaveChanges();
            }
            ExtId = Convert.ToInt32(wcs.ExternalSources.Where(e => e.source == ExtSource).Select(ext => ext.externalSourceId).FirstOrDefault());
            return ExtId;
        }
        private int AdmissionStatusId(string status)
        {
            int statusId;
            WCSEntities wcs = new WCSEntities();
            statusId = Convert.ToInt32(wcs.tbl_AdmissionStatus.Where(ast => ast.status == status).Select(admst => admst.admissionStatusId).FirstOrDefault());
            if (statusId <= 0)
            {
                tbl_AdmissionStatus adm = new tbl_AdmissionStatus();
                adm.status = status;
                wcs.tbl_AdmissionStatus.Add(adm);
                wcs.SaveChanges();
            }
            statusId = Convert.ToInt32(wcs.tbl_AdmissionStatus.Where(ast => ast.status == status).Select(admst => admst.admissionStatusId).FirstOrDefault());
            return statusId;
        }
        private bool HasAllergy(Terser tst,int obxrep,string allergytype)
        {
            ISegment segment = tst.getSegment("OBX");
            bool latexallergy = false;
            bool found = false;
            string query = "";
            bool segrepcount = segment.Message.IsRepeating("OBX");
            if (segrepcount == true)
            {
                for (int i = 0; i <= obxrep; i++)
                {
                    if (found == false)
                    {
                        string querytypefound = tst.Get("/OBX(" + i + ")-3");
                        string admallergy = tst.Get("/OBX(" + i + ")-3-2");
                        switch (allergytype)
                        {
                            case "ADM.ALLERGY":
                                query = "Latex";
                                break;
                            case "PCS.NDADM054D":
                                query = "MRSA";
                                break;
                            case "PCS.NDADM111":
                                query = "Fall";
                                break;
                        }
                         
                        
                        int latexallergyExists = Regex.Matches(admallergy, query).Count;
                        if (latexallergyExists > 0)
                        {
                            string allergyvalue = tst.Get("/OBX(" + i + ")-5");
                            if(allergyvalue=="N")
                            {
                                latexallergy = false;
                                break;
                            }
                            else
                            {
                                latexallergy = true;
                                break;
                            }
                        }
                        else
                        {
                            latexallergy=FromAL1Segment(tst, obxrep,query);
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

            }
            return latexallergy;
        }
        private bool FromAL1Segment(Terser tst,int reps,string query)
        {
            bool hasalleregen = false;
            for (int i = 0; i < reps; i++)
            {
                string allergencode = tst.Get("/AL1("+i+")-3-1");
                if(allergencode==query)
                {
                    string severitycode=tst.Get("/AL1(" + i + ")-4");
                    if(severitycode=="U")
                    {
                        hasalleregen = false;
                    }
                    else
                    {
                        hasalleregen = true;
                    }
                }
            }
            return hasalleregen;
        }
       
    }
}
