using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HL7MessageServer.Classes
{
    public static class ReturnLocation
    {
        public static string location(string loc)
        {
            string locvalue = "";
            switch(loc)
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
            if(locvalue==""||locvalue==null)
            {
                locvalue = loc;
            }
            return locvalue;
        }
    }
}