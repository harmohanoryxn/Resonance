using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Data.Odbc;
using System.Data;
using WCS.Services.IPeople.Helpers;
using WCS.Services.DataServices.Data;

namespace WCS.Services.IPeople.Activities
{
    public sealed class AddOutPatientsActivity : AddAdmissionActivity
    {
        protected override OdbcDataReader ExecuteQuery(CodeActivityContext context, OdbcConnection conn)
        {
            string admissionDate = DateTime.Now.ToString("yyyyMMdd");

            OdbcDataReader reader;

            OdbcCommand cmd = new OdbcCommand("SELECT " +
                            "r.urn AS admissionId, " + 
                            "r.statustype AS admissionType, " + 
                            "p.status AS admissionStatus, " + 
                            "p.acct AS accountId, " + 
                            "p.unitnumber AS patientId, " + 
                            "r.date AS admissionDate, " + 
                            "r.time AS admissionTime, " + 
                            "p.location AS location, " + 
                            "p.room AS room, " + 
                            "p.bed AS bed, " + 
                            "d.admitdoctorname AS admittingDoctor, " + 
                            "d.attenddoctorname AS attendingDoctor, " + 
                            "d.primcaredoctorname AS primaryCareDoctor " + 
                            "FROM  " + 
                            "(((LIVE.ADMGLY.admpatregistrationindex as r inner join LIVE.ADMGLY.admpatientfile as p on r.urn = p.urn)  " +
                            "inner join LIVE.ADMGLY.admpatdoctors as d on p.urn = d.urn " +
                            "WHERE  r.date = ? AND r.statustype != 'IN'", conn);

            cmd.Parameters.Add("@admissionDate", OdbcType.VarChar, 255).Value = admissionDate;

            reader = cmd.ExecuteReader(CommandBehavior.Default);

            return reader;
        }
    }
}
