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
    public sealed class UpdateORCasesActivity : IPeopleActivity
    {
        protected override int ExecuteUpdate(CodeActivityContext context, OdbcDataReader reader)
        {
            int orderCount = 1;

            DateTime timestamp = DateTime.Now;

            var iPeopleDataRepository = new ServerFacade();

            while (reader.Read())
            {
                string orderId = DataHelper.TryParseString(reader, "orderId");
                string orderNumber = DataHelper.TryParseString(reader, "orderNumber");
                string department = DataHelper.TryParseString(reader, "department");
                string admissionId = DataHelper.TryParseString(reader, "admissionId");
                string procedureCategory = DataHelper.TryParseString(reader, "procedureCategory");
                string procedureId = DataHelper.TryParseString(reader, "procedureId");
                string procedureCode = DataHelper.TryParseString(reader, "procedureCode");
                string procedureDescription = DataHelper.TryParseString(reader, "procedureDescription");
                string serviceDate = DataHelper.TryParseString(reader, "serviceDate");
                string serviceTime = DataHelper.TryParseString(reader, "serviceTime");
                string orderingDoctor = DataHelper.TryParseString(reader, "orderingDoctor");
                string procedureStartDate = string.Empty;
                string procedureStartTime = string.Empty;
                string completedDate = string.Empty;
                string completedTime = string.Empty;

                int? estimatedDuration = DataHelper.TryParseInt(reader, "estimatedDuration");

                int sourceId = 3;

                string priority = "U";
                string status = "U";

                iPeopleDataRepository.AddIPeopleStagingOrder(timestamp, orderId, orderNumber, department, admissionId, priority, status, procedureCategory, procedureId, procedureCode, procedureDescription, serviceDate, serviceTime, orderingDoctor, completedDate, completedTime, procedureStartDate, procedureStartTime, estimatedDuration, sourceId);

                orderCount += 1;

                iPeopleDataRepository.CommitChangesToTheDatabase();
            }

            return orderCount;
        }

        protected override OdbcDataReader ExecuteQuery(CodeActivityContext context, OdbcConnection conn)
        {
            string orderDate = DateTime.Now.ToString("yyyyMMdd");

            OdbcDataReader reader;

            OdbcCommand cmd = new OdbcCommand("SELECT OR.URN AS orderId, " +
                                        "OR.ORCOUNTER AS orderNumber, " +
                                        "OR.OPERATIONROOM AS department,  " +
                                        "OR.OPERATIONDATE AS serviceDate,  " +
                                        "OR.OPERATIONTIME AS serviceTime,  " +
                                        "OR.ESTTIME AS estimatedDuration,  " +
                                        "OR.SSPATIENT AS patientId,  " +
                                        "OR.ACCTNUMBER AS admissionId,  " +
                                        "OR.PATIENTDOCTOR AS orderingDoctor, " +
                                        "OR.PrimaryOp AS procedureId, " +
                                        "OR.PrimaryOp AS procedureCode, " +
                                        "SA.Group AS procedureCategory, " +
                                        "SA.Description AS procedureDescription " +
                                        "FROM ((LIVE.SCHGLY.OrCasesByDateX I " +
                                        "INNER JOIN LIVE.SCHGLY.OrCases AS OR ON I.URN = OR.URN) " +
                                        "INNER JOIN LIVE.SCHGLY.SchApptType SA ON SA.MNEMONIC = OR.PrimaryOp) " +
                                        "WHERE I.OPERATIONDATE = ?", conn);

            cmd.Parameters.Add("@OrderDate", OdbcType.VarChar, 255).Value = orderDate;

            reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess);

            return reader;
        }
    }
}
