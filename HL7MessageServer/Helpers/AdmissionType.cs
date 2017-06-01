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
     
    }
}
