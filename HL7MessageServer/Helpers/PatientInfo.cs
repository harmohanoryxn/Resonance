using NHapi.Base.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.Migrations;
using HL7MessageServer.Model;
using HL7MessageServer.Helpers;

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
            
                extPID = tst.Get("/PID-2");
                patientIdCheck = wcs.Patient_tbl.Where(p => p.PID == extPID).Select(pa => pa.patientId).FirstOrDefault();
            
             
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
                patienttbl.PID = extPID;
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
                wcs.SaveChanges();
                patientId = wcs.Patient_tbl.Where(p => p.externalId == extPID).Select(pa => pa.patientId).FirstOrDefault();
                Insertionhelper.insertdata("Patient", patientId, "Insertion");
            }
            else
            {
                Patient_tbl pd = wcs.Patient_tbl.First(p => p.patientId == patientIdCheck);

               
                

            }

            patientId = wcs.Patient_tbl.Where(p => p.externalId == extPID).Select(pa => pa.patientId).FirstOrDefault();
            return patientId;

        }
    }
}
