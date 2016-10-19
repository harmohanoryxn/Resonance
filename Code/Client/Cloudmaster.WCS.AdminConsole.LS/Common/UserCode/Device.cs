using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.LightSwitch;
namespace LightSwitchApplication
{
    public partial class Device
    {
        partial void lockTimeout_Validate(EntityValidationResultsBuilder results)
        {
            if (lockTimeout < 0)
            {
                results.AddPropertyError("Lock timeout seconds must be greater than zero.");
            }
        }
    }
}
