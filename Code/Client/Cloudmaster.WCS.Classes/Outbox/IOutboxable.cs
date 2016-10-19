using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cloudmaster.WCS.Classes
{
    public static class OutboxStatus
    {
        public static string Pending = "Pending";
        public static string Sending = "Sending";
        public static string Completed = "Completed";
        public static string Error = "Error";
    }

    public interface IOutboxable
    {
        string CreateUniqueFilename();

        string OutboxStatus { set; get; }
    }
}
