using HL7MessageServer.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;
using System.Web;

namespace HL7MessageServer.Helpers
{
    public static class Insertionhelper
    {
        public static void insertdata(string tblname,int recordid,string type)
        {

            //DateTime currentdate = DateTime.Now;
            //string content = "Table Effected: " + tblname + "              Record Id :" + recordid + "              DatetimeInserted:" + currentdate+"       Type: "+type;
            //string directory = ConfigurationManager.AppSettings["Insertion"];
            //System.IO.File.WriteAllText(directory + currentdate.ToString() + ".txt", content);
            WCSHL7Entities wcs = new WCSHL7Entities();
            Update up = new Update();
            up.type = type;
            up.source = "HL7 Message";
            
            int externalIdcheck = wcs.Updates.Where(c => c.value == tblname && (c.Order_orderId==recordid || c.Admission_admissionId==recordid)).Select(ad => ad.updateId).FirstOrDefault();
            if (externalIdcheck > 0)
            {
                
                Update ad = wcs.Updates.First(c => c.value == tblname && (c.Order_orderId == recordid || c.Admission_admissionId == recordid));
                ad.type = type;
                ad.source = "HL7 Message";
                if (type == "Order Imported" || type == "Order Updated")
                {
                    ad.Order_orderId = recordid;
                    ad.value = tblname;
                }
                else if (type == "Admission Imported" || type == "Admission Updated")
                {
                    ad.Admission_admissionId = recordid;
                    ad.value = tblname;
                }
                ad.dateCreated = DateTime.Now;
                wcs.SaveChanges();
            }
            else
            {
                if (type == "Order Imported" || type == "Order Updated")
                {
                    up.Order_orderId = recordid;
                    up.value = tblname;
                }
                else if (type == "Admission Imported" || type == "Admission Updated")
                {
                    up.Admission_admissionId = recordid;
                    up.value = tblname;
                }
                up.dateCreated = DateTime.Now;
                wcs.Updates.Add(up);
                wcs.SaveChanges();
            }
            
            

        }
    }
}