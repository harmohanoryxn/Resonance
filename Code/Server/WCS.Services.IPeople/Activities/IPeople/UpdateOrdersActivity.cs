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
    public sealed class UpdateOrdersActivity : IPeopleActivity
    {
        protected override int ExecuteUpdate(CodeActivityContext context, OdbcDataReader reader)
        {
            DateTime timestamp = DateTime.Now;

            int orderCount = 0;

            var iPeopleDataRepository = new ServerFacade();

            while (reader.Read())
            {
                string orderId = DataHelper.TryParseString(reader, "orderId");
                string orderNumber = DataHelper.TryParseString(reader, "orderNumber");
                string department = DataHelper.TryParseString(reader, "department");
                string admissionId = DataHelper.TryParseString(reader, "admissionId");
                string priority = DataHelper.TryParseString(reader, "priority");
                string status = DataHelper.TryParseString(reader, "status");
                string procedureCategory = DataHelper.TryParseString(reader, "procedureCategory");
                string procedureId = DataHelper.TryParseString(reader, "procedureId");
                string procedureCode = DataHelper.TryParseString(reader, "procedureCode");
                string procedureDescription = DataHelper.TryParseString(reader, "procedureDescription");
                string serviceDate = DataHelper.TryParseString(reader, "serviceDate");
                string serviceTime = DataHelper.TryParseString(reader, "serviceTime");
                string orderingDoctor = DataHelper.TryParseString(reader, "orderingDoctor");
                string procedureStartDate = DataHelper.TryParseString(reader, "procedureStartDate");
                string procedureStartTime = DataHelper.TryParseString(reader, "procedureStartTime");
                string completedDate = DataHelper.TryParseString(reader, "completedDate");
                string completedTime = DataHelper.TryParseString(reader, "completedTime");

                int sourceId = 1;

                iPeopleDataRepository.AddIPeopleStagingOrder(timestamp, orderId, orderNumber, department, admissionId, priority, status, procedureCategory, procedureId, procedureCode, procedureDescription, serviceDate, serviceTime, orderingDoctor, completedDate, completedTime, procedureStartDate, procedureStartTime, null, sourceId);

                iPeopleDataRepository.CommitChangesToTheDatabase();

                orderCount += 1;
            }

            return orderCount;
        }

        protected override OdbcDataReader ExecuteQuery(CodeActivityContext context, OdbcConnection conn)
        {
            string orderDate = DateTime.Now.ToString("yyyyMMdd");

            OdbcDataReader reader;

            OdbcCommand cmd = new OdbcCommand("SELECT " +
                            "O.URN AS orderId, " +
                            "O.OrderNum AS orderNumber, " +
                            "O.CATEGORY AS department, " +
                            "O.Patient AS admissionId, " +
                            "O.Priority AS priority, " +
                            "O.Status AS status, " +
                            "P.Category AS procedureCategory, " +
                            "P.Number AS procedureId, " +
                            "P.Mnemonic AS procedureCode, " +
                            "P.Name AS procedureDescription, " +
                            "O.ServiceDate AS serviceDate, " +
                            "O.ServiceTime AS serviceTime, " +
                            "O.Doctor AS orderingDoctor, " +
                            "SD.InProDate AS procedureStartDate, " +
                            "SD.InProTime AS procedureStartTime, " +
                            "SD.CompDate AS completedDate, " +
                            "SD.CompTime AS completedTime " +
                            "FROM (((LIVE.OEGLY.OeOrder as O INNER JOIN LIVE.OEGLY.OeOrdsByPatAndOrdDateX as OI on OI.URN = O.URN) " +
                            "LEFT JOIN LIVE.OEGLY.OeOrderStatusData AS SD ON OI.URN = SD.URN) " +
                            "INNER JOIN LIVE.OEGLY.OeProcDict AS P on P.Number = O.Procedure AND P.Category = O.CATEGORY) " +
                            "WHERE OI.ORDERDATE = ?", conn);

            cmd.Parameters.Add("@OrderDate", OdbcType.VarChar, 255).Value = orderDate;

            reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess);

            return reader;
        }
    }
}
