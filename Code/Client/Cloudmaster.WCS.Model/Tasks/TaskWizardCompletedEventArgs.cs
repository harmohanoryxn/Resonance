using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cloudmaster.WCS.Model.Tasks
{
    public class TaskWizardCompletedEventArgs : EventArgs 
    {
        public bool Cancelled { get; set; }

        public string TaskId { get; set; }
    }
}
