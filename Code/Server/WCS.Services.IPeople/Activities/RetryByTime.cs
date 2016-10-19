using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Activities.Statements;
using System.ComponentModel;
using System.Windows.Markup;
using System.Activities.Validation;
using System.Diagnostics;
using System.Windows;

namespace WCS.Services.IPeople
{
    [Designer(typeof(RetryByTimeActivityDesigner))]
    public class RetryByTime : NativeActivity
    {
        public InArgument<TimeSpan> MaxTime { get; set; }
        public InArgument<TimeSpan> RetryDelay { get; set; }

        private Delay Delay = new Delay();

        public Activity Body { get; set; }

        public OutArgument<DateTime> StartTime { get; set; }
        public OutArgument<int> Attempts { get; set; }

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            metadata.AddChild(Body);

            RuntimeArgument maxTimeArg = new RuntimeArgument("MaxTime", typeof(TimeSpan), ArgumentDirection.In);
            metadata.Bind(MaxTime, maxTimeArg);
            metadata.AddArgument(maxTimeArg);

            RuntimeArgument attemptsArg = new RuntimeArgument("Attempts", typeof(int), ArgumentDirection.Out);
            metadata.Bind(Attempts, attemptsArg);
            metadata.AddArgument(attemptsArg);

            Delay.Duration = RetryDelay;

            metadata.AddImplementationChild(Delay);
        }

        protected override void Execute(NativeActivityContext context)
        {
            Attempts.Set(context, 1);

            context.ScheduleActivity(Body, OnFaulted);
        }

        private void OnFaulted(NativeActivityFaultContext faultContext, Exception propagatedException, ActivityInstance propagatedFrom)
        {
            int attempts = Attempts.Get(faultContext);

            DateTime startTime = StartTime.Get(faultContext);
            TimeSpan maxTime = MaxTime.Get(faultContext);

            TimeSpan timeSinceStartOfActivty = DateTime.Now.Subtract(startTime);

            if (timeSinceStartOfActivty < maxTime)
            {
                faultContext.CancelChild(propagatedFrom);
                faultContext.HandleFault();
                faultContext.ScheduleActivity(Delay, OnDelayComplete);

                Attempts.Set(faultContext, attempts + 1);
            }
            else
            {
                string errorMessage = string.Format("Activity continously failed for {0}", timeSinceStartOfActivty);

                throw new RetryByTimeTimeoutException(errorMessage);
            }
        }

        private void OnDelayComplete(NativeActivityContext context, ActivityInstance completedInstance)
        {
            context.ScheduleActivity(Body, OnFaulted);
        }
    }

}
