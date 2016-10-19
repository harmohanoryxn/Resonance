using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Cloudmaster.WCS.Classes;
using Cloudmaster.WCS.Classes.Helpers;

namespace Cloudmaster.WCS.Controls.Helpers
{
    public class OrdersHelper
    {
        public static bool RequiresFasting(Order order)
        {
            bool result = false;

            double priorFastingHours = GetPriorFastingHours(order);

            if ((priorFastingHours > 0) && (order.RequestedDateTime.HasValue))
            {
                result = true;
            }

            return result;
        }

        public static bool IsCurrentlyFasting(Order order)
        {
            bool result = false;

            double priorFastingHours = GetPriorFastingHours(order);

            if ((priorFastingHours > 0) && (order.RequestedDateTime.HasValue))
            {
                DateTime now = DateTime.Now;

                DateTime requestedDateTime = order.RequestedDateTime.Value;
                DateTime startFastingTime = requestedDateTime.AddHours(-priorFastingHours);

                if ((now > startFastingTime) && (now < requestedDateTime))
                {
                    result = true;
                }
            }

            return result;
        }

        public static string GetShortStartTime(Order order)
        {
            string result = string.Empty;

            double priorFastingHours = GetPriorFastingHours(order);

            if (priorFastingHours == 24)
            {
                result = "Midnight";
            }
            else
            {
                if (order.RequestedDateTime.HasValue)
                {
                    DateTime requestedDateTime = order.RequestedDateTime.Value;
                    DateTime startFastingTime = requestedDateTime.AddHours(-priorFastingHours);

                    result = string.Format("{0:t}", startFastingTime);
                }
            }

            return result;
        }

        public static double GetPriorFastingHours(Order order)
        {
            double result = 0;

            string trimmedServiceText = order.ServiceText.Trim();

            if (order.Service == "FLUORO")
            {
                if ((trimmedServiceText == "BR SWALL") || (trimmedServiceText == "BT MEAL") || (trimmedServiceText == "BARIUM FT"))
                {
                    result = 8;
                }
            }
            else if (order.Service == "CT")
            {
                if ((trimmedServiceText == "ABD PELVW"))
                {
                    result = 4;
                }
            }
            else if (order.Service == "US")
            {
                if ((trimmedServiceText == "ABD US"))
                {
                    result = 12;
                }
            }
            else if (order.Service == "MRI")
            {
                if ((trimmedServiceText.Trim() == "ABD") || (trimmedServiceText == "ENTERO"))
                {
                    result = 7;
                }
                else if ((trimmedServiceText == "PE") || (trimmedServiceText == "MR ANGIO"))
                {
                    result = 4;
                }
            }
            else if (order.Service == "IR")
            {
                result = 24;
            }

            return result;
        }
        public static string GetDescriptionForService(string service)
        {
            string result = service;

            if (service == "FLUORO")
            {
                result = "Fluoroscopic";
            }
            else if (service == "CT")
            {
                result = "C.T. Scan";
            }
            else if (service == "US")
            {
                result = "Ultrasound";
            }
            else if (service == "MRI")
            {
                result = "M.R.I.";
            }
            else if (service == "NM")
            {
                result = "Nuclear Medicine";
            }
            else if (service == "IR")
            {
                result = "I.R.";
            }

            return result;
        }

        public static string GetDescriptionForServiceText(string service, string serviceText)
        {
            string result = serviceText;

            if (service == "FLUORO")
            {
                if (serviceText == "BR SWALL")
                {
                    result = "Barium Swallow";
                }
                else if (serviceText == "BR MEAL")
                {
                    result = "Barium Meal";
                }
                else if (serviceText == "BR FT")
                {
                    result = "Barium Follow Through";
                }
                else if (serviceText == "BR ENEMA")
                {
                    result = "Barium Enema";
                }
            }
            else if (service == "CT")
            {
                if (serviceText == "ABD PEL VW")
                {
                    result = "Chest/Abdomen/Pelvis";
                }
            }
            else if (service == "US")
            {
                if (serviceText == "RENAL")
                {
                    result = "Renal Ultrasound";
                }
                else if (serviceText == "ABD US")
                {
                    result = "Abdominal Ultrasound";
                }
                else if (serviceText == "PEL")
                {
                    result = "Pelvis Ultrasound";
                }
            }
            else if (service == "MRI")
            {
                if (serviceText == "ABD")
                {
                    result = "M.R.I. Abdomen";
                }
                else if (serviceText == "PE")
                {
                    result = "M.R.I. Pelvis";
                }
                else if (serviceText == "ENTERO")
                {
                    result = "M.R.I. Enteroclysis";
                }
                else if (serviceText == "BR")
                {
                    result = "M.R.I. Breast";
                }
                else if (serviceText == "MR ANGIO")
                {
                    result = "Renal M.R.A.";
                }
                else if (serviceText == "CAR")
                {
                    result = "Carotid M.R.A.";
                }
            }
            else if (service == "NM")
            {
                if (serviceText == "BON")
                {
                    result = "Isotope Bone Scans";
                }
                else if (serviceText == "RNFO")
                {
                    result = "Renogram";
                }
                else if (serviceText == "THYT")
                {
                    result = "Isotope Thyroid Scan";
                }
                else if (serviceText == "PARA")
                {
                    result = "Parathyroid";
                }
                else if (serviceText == "LNGP")
                {
                    result = "Perfusion Lung Scans";
                }
            }
            else if (service == "IR")
            {
                if (serviceText == "PC")
                {
                    result = "Portacath Implantation";
                }
                else if (serviceText == "CT BX MO")
                {
                    result = "Biopsies";
                }
                else if (serviceText == "ANGIO LOWR")
                {
                    result = "Peripheral Angiography";
                }
            }

            return result;
        }
    }
}
