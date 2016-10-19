using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cloudmaster.WCS.Processing;

namespace Cloudmaster.WCS.Department.Processing
{
    public class UpdateRequestedDateTimeOverrideArguements : ProcessorArguements
    {
        public string OrderNumber { get; set; }

        public DateTime? RequestedDateTimeOverride { get; set; }

        public string Notes { get; set; }
    }
}
