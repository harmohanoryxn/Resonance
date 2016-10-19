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
    public abstract class AddAdmissionActivity : IPeopleActivity
    {
        private static int admissionCount = 1;

        protected override int ExecuteUpdate(CodeActivityContext context, OdbcDataReader reader)
        {
            DateTime timestamp = DateTime.Now;

            var iPeopleDataRepository = new ServerFacade();

            while (reader.Read())
            {
                string admissionId = DataHelper.TryParseString(reader, "admissionId");
                string admissionType = DataHelper.TryParseString(reader, "admissionType");
                string admissionStatus = DataHelper.TryParseString(reader, "admissionStatus");
                string accountId = DataHelper.TryParseString(reader, "accountId");
                string patientId = DataHelper.TryParseString(reader, "patientId");
                string admissionDate = DataHelper.TryParseString(reader, "admissionDate");
                string admissionTime = DataHelper.TryParseString(reader, "admissionTime");
                string location = DataHelper.TryParseString(reader, "location");
                string room = DataHelper.TryParseString(reader, "room");
                string bed = DataHelper.TryParseString(reader, "bed");
                string admittingDoctor = DataHelper.TryParseString(reader, "admittingDoctor");
                string attendingDoctor = DataHelper.TryParseString(reader, "attendingDoctor");
                string primaryCareDoctor = DataHelper.TryParseString(reader, "primaryCareDoctor");

                iPeopleDataRepository.AddIPeopleStagingAdmission(admissionCount, timestamp, admissionId, admissionType, admissionStatus, accountId, patientId, admissionDate, admissionTime, location, room, bed, string.Empty, string.Empty, string.Empty);

                admissionCount += 1;
            }

            iPeopleDataRepository.CommitChangesToTheDatabase();

            return admissionCount;
        }
    }
}
