using NHapi.Base.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.Migrations;
using HL7MessageServer.Model;
using HL7MessageServer.Helpers;
using System.Data.Entity.Validation;

namespace H7Message
{
    public static class PatientInfo
    {
         public static int PatientinfoReturn(Terser tst, int obxrep)
        {
            WCSHL7Entities wcs = new WCSHL7Entities();
            string extPID = tst.Get("/PID-3");
            string extpid = "";
            var messageevent = tst.Get("/MSH-9-2");
            int patientIdCheck = 0;
            
                extPID = tst.Get("/PID-3");
                patientIdCheck = wcs.Patient_tbl.Where(p => p.externalId == extPID).Select(pa => pa.patientId).FirstOrDefault();
            
             
            int patientId = 0;
            if (patientIdCheck == 0)
            {
                string patientextId = "";
                Patient_tbl patienttbl = new Patient_tbl();
                if (messageevent=="A01" ||messageevent=="A04")
                {
                    patientextId = tst.Get("/PID-3");
                    patienttbl.externalId = patientextId;
                }
                else
                {
                    patienttbl.externalId = extPID;
                }
               
                patienttbl.externalSourceId = 1;
               
                patienttbl.givenName = tst.Get("/PID-5-2");
                patienttbl.surname = tst.Get("/PID-5");
               
                var dob = DateTime.ParseExact(tst.Get("/PID-7"), "yyyyMMdd", null).ToString("yyyy-MM-dd HH:mm");
                patienttbl.dob = Convert.ToDateTime(dob);
                patienttbl.sex = tst.Get("/PID-8");
                ////Needs to be checked///
                patienttbl.isAssistanceRequired = AllergenDetails.HasAllergy(tst, obxrep, "Assistance"); ;
                patienttbl.isFallRisk = AllergenDetails.HasAllergy(tst, obxrep, "PCS.NDADM111"); ;
                patienttbl.isMrsaPositive = AllergenDetails.HasAllergy(tst, obxrep, "PCS.NDADM054D"); ;
                patienttbl.assistanceDescription = "";
                patienttbl.hasLatexAllergy = AllergenDetails.HasAllergy(tst, obxrep, "ADM.Allergy");
                wcs.Patient_tbl.Add(patienttbl);
                try
                {
                    wcs.SaveChanges();
                    patientId = wcs.Patient_tbl.Where(p => p.externalId == extPID).Select(pa => pa.patientId).FirstOrDefault();
                    Insertionhelper.insertdata("Patient", patientId, "Patient Insertion");
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (DbEntityValidationResult entityValidationError in ex.EntityValidationErrors)
                    {
                        string message = "Entity of type " + entityValidationError.Entry.Entity.GetType().Name + " in state " + (object)entityValidationError.Entry.State + "has the following validation errors:";
                        foreach (DbValidationError validationError in (IEnumerable<DbValidationError>)entityValidationError.ValidationErrors)
                            HL7messageToFile.Exceptionhandler(message, validationError.ErrorMessage);
                    }
                }
              
            }
            else
            {
                Patient_tbl pd = wcs.Patient_tbl.First(p => p.patientId == patientIdCheck);
                pd.givenName = tst.Get("/PID-5-2");
                pd.surname = tst.Get("/PID-5");

                var dob = DateTime.ParseExact(tst.Get("/PID-7"), "yyyyMMdd", null).ToString("yyyy-MM-dd HH:mm");
                pd.dob = Convert.ToDateTime(dob);
                pd.sex = tst.Get("/PID-8");
                ////Needs to be checked///
                pd.isAssistanceRequired = AllergenDetails.HasAllergy(tst, obxrep, "Assistance"); ;
                pd.isFallRisk = AllergenDetails.HasAllergy(tst, obxrep, "PCS.NDADM111"); ;
                pd.isMrsaPositive = AllergenDetails.HasAllergy(tst, obxrep, "PCS.NDADM054D"); ;
                pd.assistanceDescription = "";
                pd.hasLatexAllergy = AllergenDetails.HasAllergy(tst, obxrep, "ADM.Allergy");

                try
                {
                    wcs.SaveChanges();
                    Insertionhelper.insertdata("Patient Info", patientIdCheck, "Patient Updated");
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (DbEntityValidationResult entityValidationError in ex.EntityValidationErrors)
                    {
                        string message = "Entity of type " + entityValidationError.Entry.Entity.GetType().Name + " in state " + (object)entityValidationError.Entry.State + "has the following validation errors:";
                        foreach (DbValidationError validationError in (IEnumerable<DbValidationError>)entityValidationError.ValidationErrors)
                            HL7messageToFile.Exceptionhandler(message, validationError.ErrorMessage);
                    }
                }




            }

            patientId = wcs.Patient_tbl.Where(p => p.externalId == extPID).Select(pa => pa.patientId).FirstOrDefault();
            return patientId;

        }
    }
}
