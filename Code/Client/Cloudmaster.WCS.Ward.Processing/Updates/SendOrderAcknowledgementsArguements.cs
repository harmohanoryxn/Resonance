using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cloudmaster.WCS.Processing;

namespace Cloudmaster.WCS.Ward.Processing
{
    public class SendOrderAcknowledgementsArguements : ProcessorArguements
    {
        public string OrderNumber { get; set; }

        public bool IsFastingAcknowledged { get; set; }

        public bool IsPrepWorkAcknowledged { get; set; }

        public bool IsExamAcknowledged { get; set; }

        public bool IsInjectionAcknowledged { get; set; }
    }
}
