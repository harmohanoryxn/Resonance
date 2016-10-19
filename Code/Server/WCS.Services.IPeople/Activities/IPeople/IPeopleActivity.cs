using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Data.Odbc;

namespace WCS.Services.IPeople.Activities
{
    public abstract class IPeopleActivity : CodeActivity
    {
        public InArgument<string> IPeopleChannelName { get; set; }
        public InArgument<string> IPeopleConnectionString { get; set; }

        public OutArgument<TimeSpan> QueryExecutionTime { get; set; }
        public OutArgument<TimeSpan> UpdateExecutionTime { get; set; }
        public OutArgument<TimeSpan> ActivityExecutionTime { get; set; }

        public OutArgument<int> NumberOfRowsUpdated { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            Console.WriteLine("Starting activity");

            string iPeopleConnectionString = "DSN=Meditech.App.MIS";

            DateTime activityStartTime = DateTime.Now;
            DateTime queryStartTime = DateTime.Now;

            DateTime queryEndTime;
            DateTime updateStartTime;

            int numberOfRowsUpdated;

            using (OdbcConnection conn = new OdbcConnection(iPeopleConnectionString))
            {
                conn.Open();

                OdbcDataReader dataReader = ExecuteQuery(context, conn);

                queryEndTime = DateTime.Now;
                updateStartTime = DateTime.Now;

                numberOfRowsUpdated = ExecuteUpdate(context, dataReader);
            }

            DateTime updateEndTime = DateTime.Now;
            DateTime activityEndTime = DateTime.Now;

            context.SetValue(this.NumberOfRowsUpdated, numberOfRowsUpdated);

            context.SetValue(this.QueryExecutionTime, queryEndTime.Subtract(queryStartTime));
            context.SetValue(this.UpdateExecutionTime, updateEndTime.Subtract(updateStartTime));
            context.SetValue(this.ActivityExecutionTime, activityEndTime.Subtract(activityStartTime));
        }

        protected abstract OdbcDataReader ExecuteQuery(CodeActivityContext context, OdbcConnection conn);

        protected abstract int ExecuteUpdate(CodeActivityContext context, OdbcDataReader dataReader);
    }
}
