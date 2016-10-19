using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WCS.Core;

namespace Cloudmaster.WCS.DataServicesInvoker.DataServices
{
    public partial class Room : IIdentifable
    {
        public int Id
        {
            get { return RoomId; }
        }

        public int GetFingerprint()
        {
            var fp = 0;
            if (!string.IsNullOrEmpty(Ward))
                fp = fp ^ Ward.GetHashCode();
            if (!string.IsNullOrEmpty(Name))
                fp = fp ^ Name.GetHashCode();
            return fp;
        }
    }
}
