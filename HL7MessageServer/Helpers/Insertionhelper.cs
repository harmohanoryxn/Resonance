using HL7MessageServer.ErrorHandler;
using HL7MessageServer.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Linq;
using System.Reflection;
using System.Web;

namespace HL7MessageServer.Helpers
{
    public static class Insertionhelper
    {
        public static void insertdata(string tblname,int recordid,string type)
        {

            
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
                try
                {
                    wcs.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (DbEntityValidationResult entityValidationError in ex.EntityValidationErrors)
                    {
                        string message = "Entity of type " + entityValidationError.Entry.Entity.GetType().Name + " in state " + (object)entityValidationError.Entry.State + "has the following validation errors:";
                        foreach (DbValidationError validationError in (IEnumerable<DbValidationError>)entityValidationError.ValidationErrors)
                        {
                            ErrorMessage err = new ErrorMessage();
                            err.Innermessage = message;
                            err.StackTrace = validationError.ErrorMessage;
                            err.Ordernumber = recordid.ToString();
                            err.ErrorDatetime = DateTime.Now.ToShortDateString();
                        }

                    }
                }

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
                try
                {
                    wcs.Updates.Add(up);

                    wcs.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (DbEntityValidationResult entityValidationError in ex.EntityValidationErrors)
                    {
                        string message = "Entity of type " + entityValidationError.Entry.Entity.GetType().Name + " in state " + (object)entityValidationError.Entry.State + "has the following validation errors:";
                        foreach (DbValidationError validationError in (IEnumerable<DbValidationError>)entityValidationError.ValidationErrors)
                        {
                            ErrorMessage err = new ErrorMessage();
                            err.Innermessage = message;
                            err.StackTrace = validationError.ErrorMessage;
                            err.Ordernumber = recordid.ToString();
                            err.ErrorDatetime = DateTime.Now.ToShortDateString();
                        }
                            
                    }
                }
            }
            
            

        }
    }
}