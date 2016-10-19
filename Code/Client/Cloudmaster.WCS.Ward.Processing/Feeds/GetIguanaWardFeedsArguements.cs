using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cloudmaster.WCS.Processing;

namespace Cloudmaster.WCS.Ward.Processing
{
    public class GetIguanaWardFeedsArguements : ProcessorArguements
    {
        public string WardIdentifier { get; set; }
    }
}
