using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cloudmaster.WCS.Classes
{
    public interface IAutoSaveable
    {
        string CreateUniqueFilename();

        DateTime LastSaved { set; get; }
    }
}
