using HL7MessageServer.Model;
using NHapi.Base.Model;
using NHapi.Base.Util;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace H7Message
{
    public static class AdmissionType
    {
        public static int AdmissionTypeId(Terser tst)
        {
            WCSHL7Entities wcs = new WCSHL7Entities();
            string admissiontype = tst.Get("/PV1-18");
            if (admissiontype == "" || admissiontype == null)
            {
                admissiontype = tst.Get("/PV1-2");
                switch (admissiontype)
                {
                    case "P":
                        admissiontype = "Out";
                        break;
                    case "I":
                        admissiontype = "In";
                        break;
                    case "E":
                        admissiontype = "Out";
                        break;
                    case "O":
                        admissiontype = "Out";
                        break;
                    case "":
                        admissiontype = "Day";
                        break;
                }
                int admissionTypeId = Convert.ToInt32(wcs.tbl_AdmissionType.Where(a => a.type == admissiontype).Select(ad => ad.admissionTypeId).FirstOrDefault());
                return admissionTypeId;
            }
            else
            {
                switch (admissiontype)
                {
                    case "CLI":
                        admissiontype = "Out";
                        break;
                    case "ER":
                        admissiontype = "Out";
                        break;
                    case "IN":
                        admissiontype = "In";
                        break;
                    case "INO":
                        admissiontype = "In";
                        break;
                    case "RCR":
                        admissiontype = "Out";
                        break;
                    case "REF":
                        admissiontype = "Out";
                        break;
                    case "SDC":
                        admissiontype = "Day";
                        break;
                }
            
                int admissionTypeId = Convert.ToInt32(wcs.tbl_AdmissionType.Where(a => a.type == admissiontype).Select(ad => ad.admissionTypeId).FirstOrDefault());
                return admissionTypeId;
            }
            
            
        }
        //public static int AdmissionTypeId(Terser tst)
        //{
        //    WCSHL7Entities wcs = new WCSHL7Entities();
        //    string admissiontype = tst.Get("/PV1-18");
        //    if (admissiontype == "" || admissiontype == null)
        //    {

        //    }
        //    else
        //    {
        //        int admissionTypecheck = Convert.ToInt32(wcs.tbl_AdmissionType.Where(a => a.type == admissiontype).Select(ad => ad.admissionTypeId).FirstOrDefault());
        //        if (admissionTypecheck <= 0)
        //        {
        //            tbl_AdmissionType admtype = new tbl_AdmissionType();
        //            admtype.type = admissiontype;
        //            wcs.tbl_AdmissionType.Add(admtype);
        //            try
        //            {
        //                wcs.SaveChanges();
        //            }
        //            catch (DbEntityValidationException e)
        //            {

        //                foreach (var eve in e.EntityValidationErrors)
        //                {
        //                    var exception = "Entity of type " + eve.Entry.Entity.GetType().Name + " in state " + eve.Entry.State + "has the following validation errors:";
        //                    foreach (var ve in eve.ValidationErrors)
        //                    {
        //                        var errors = " Property: " + ve.PropertyName + ", Error: " + ve.ErrorMessage;
        //                    }
        //                }
        //                throw;
        //            }
        //        }
        //    }
        //    int admissionTypeId = Convert.ToInt32(wcs.tbl_AdmissionType.Where(a => a.type == admissiontype).Select(ad => ad.admissionTypeId).FirstOrDefault());
        //    return admissionTypeId;
        //}
    }
}
