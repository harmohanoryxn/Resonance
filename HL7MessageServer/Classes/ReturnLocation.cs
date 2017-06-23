using HL7MessageServer.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace HL7MessageServer.Classes
{
    public static class ReturnLocation
    {
       
        public static string location(string loc,int procedureid)
        {
            string ActiveHospital = ConfigurationManager.AppSettings["ActiveHospital"];
            WCSHL7Entities wcs = new WCSHL7Entities();
            string locvalue = "";
            if (ActiveHospital == "Galway")
            {
                switch (loc)
                {
                    case "MTERESA":
                        locvalue = "Mother Teresa";
                        break;
                    case "ADM WA":
                        locvalue = "Admissions Waiting Area";
                        break;
                    case "OMALLEY":
                        locvalue = "O Malley";
                        break;
                    case "JOHN PAUL2":
                        locvalue = "John Paul II";
                        break;
                    case "FLORENCE":
                        locvalue = "Florence Nightingale";
                        break;
                    case "OLOK":
                        locvalue = "Our Lady Of Knock";
                        break;
                    case "XR":
                        locvalue = "X-Ray";
                        break;
                    case "FL":
                        locvalue = "Fluoroscopy";
                        break;
                    case "THEATRE":
                        locvalue = "Theatre 1";
                        break;
                    case "CCU":
                        locvalue = "Cardiology";
                        break;
                    case "NUC MED":
                        locvalue = "Nuclear Medicine";
                        break;
                    case "INTER RAD":
                        locvalue = "X-Ray";
                        break;
                    case "IMAGING":
                        locvalue = "X-Ray";
                        break;

                    case "A&E":
                        locvalue = "Emergency Room";
                        break;
                    case "A and E":
                        locvalue = "Emergency Room";
                        break;
                    case "AE":
                        locvalue = "Emergency Room";
                        break;
                    case "PET":
                        locvalue = "Nuclear Medicine";
                        break;
                    case "ER OVERFL":
                        locvalue = "Emergency Room Overflow";
                        break;
                    case "CARD NON I":
                        locvalue = "Cardiology";
                        break;
                    case "SURG DAY":
                        locvalue = "Day Ward";
                        break;
                    case "CT ONC":
                        locvalue = "CT Scan";
                        break;
                    case "ER":
                        locvalue = "Emergency Room";
                        break;
                    case "IR":
                        locvalue = "Fluoroscopy";
                        break;
                }
            }
            else if(ActiveHospital=="Beacon")
            {
                switch (loc)
                {
                    case "3FLOOR": locvalue = "3FLOOR"; break;
                    case "3FLOOR VB": locvalue = "3RD FLOOR VIRTUAL BED"; break;
                    case "4FLOOR VB": locvalue = "4TH FLR VIRTUAL BED(NTH & STH)"; break;
                    case "4SOUTH": locvalue = "4SOUTH"; break;
                    case "5FLOOR": locvalue = "5FLOOR"; break;
                    case "5FLOOR VB": locvalue = "5TH FLOOR VIRTUAL BED"; break;
                    case "6FLOOR": locvalue = "6FLOOR"; break;
                    case "6FLOOR VB": locvalue = "6TH FLOOR VIRTUAL BED"; break;
                    case "BARIATRIC": locvalue = "BARIATRIC CLINIC"; break;
                    case "BREASTCLIN": locvalue = "BREAST CARE CLINIC"; break;
                    case "CARD": locvalue = "CARDIOLOGY"; break;
                    case "CHESTPAIN": locvalue = "CHEST PAIN CLINIC"; break;
                    case "CONS": locvalue = "CONSULTANT SUITES"; break;
                    case "CRTCRE": locvalue = "CRITICAL CARE"; break;
                    case "DAYCARE": locvalue = "DAYCARE"; break;
                    case "DC": locvalue = "DAY CARE UNIT"; break;
                    case "DC ENT-5": locvalue = "DAYCARE ENT 5TH FLOOR"; break;
                    case "DC SAT-3": locvalue = "DC SATELLITE CLINIC 3RD FLOOR"; break;
                    case "DCONC": locvalue = "DAY UNIT ONCOLOGY"; break;
                    case "DIABETES": locvalue = "DIABETES CENTRE"; break;
                    case "DIET": locvalue = "CLINICAL NUTRITION DEPARTMENT"; break;
                    case "DRESSING": locvalue = "DRESSING CLINIC"; break;
                    case "DROGHEDA": locvalue = "BEACON DROGHEDA O/PAT"; break;
                    case "END": locvalue = "ENDOSCOPY"; break;
                    case "HEALTHCHCK": locvalue = "BEACON HEALTH CHECK"; break;
                    case "HEALTHCHRV": locvalue = "BEACON HEALTHCHECK REVIEW"; break;
                    case "LAB": locvalue = "LABORATORY"; break;
                    case "LAB CENSUS": locvalue = "LAB CENSUS FACILITY"; break;
                    case "MEDSURG": locvalue = "4NORTH"; break;
                    case "MULLINGAR": locvalue = "BEACON MULLINGAR O/PAT"; break;
                    case "NEURO": locvalue = "NEUROPHYSIOLOGY DEPT"; break;
                    case "NTPF": locvalue = "HSE PUBLIC REFERRALS"; break;
                    case "NTPFCS": locvalue = "NTPF CONSULTANT SUITES"; break;
                    case "OCCHEALTH": locvalue = "OCCUPATIONAL HEALTH"; break;
                    case "OT": locvalue = "OCCUPATIONAL THERAPY"; break;
                    case "OUTPAT": locvalue = "OUT PATIENTS"; break;
                    case "OUTPATOR": locvalue = "ORTHOPAEDIC CLINIC"; break;
                    case "PAEDSOP": locvalue = "BEACON PAEDIATRIC OP SERVICE"; break;
                    case "PT": locvalue = "PHYSIOTHERAPY DEPARTMENT"; break;
                    case "RAD":
                        if (procedureid == 0)
                        {
                            locvalue = "Radiology";
                        }
                        else
                        {
                            locvalue = wcs.ProcedureCategories.Where(c => c.procedureCategoryId == (wcs.Procedures.Where(p => p.procedureId == procedureid).Select(pid => pid.ProcedureCategory_procedureCategoryId).FirstOrDefault())).Select(pcid => pcid.description).FirstOrDefault();
                        }
                        break;
                    case "RADIO": locvalue = "RADIOTHERAPY"; break;
                    case "RT": locvalue = "RESPIRATORY THERAPY"; break;
                    case "SLT": locvalue = "SPEECH AND LANGUAGE THERAPY"; break;
                    case "THEATRE": locvalue = "THEATRE"; break;
                    case "URGENT": locvalue = "URGENT CARE"; break;
                    case "URGENTREV": locvalue = "URGENT CARE REVIEW"; break;
                    case "UROLOGY": locvalue = "UROLOGY PROCEDURE ROOM"; break;
                    case "VASCLAB": locvalue = "VASCULAR LAB"; break;
                    case "WELLCL": locvalue = "WELL CLINIC"; break;
                    case "WEXFORD OP": locvalue = "BEACON WEXFORD OUTPATIENT"; break;
                    case "WOMENCEN":locvalue = "WOMENS CENTRE";break;
                }
            }
            


           
            if(locvalue==""||locvalue==null)
            {
                locvalue = loc;
            }
            return locvalue;
        }
    }
}