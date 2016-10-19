using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.LightSwitch;
namespace LightSwitchApplication
{
    public partial class NotificationRule
    {
        partial void priorToProcedureTime_Validate(EntityValidationResultsBuilder results)
        {
            if (priorToProcedureTime < 0)
            {
                results.AddPropertyError("Value must be greater than zero.");
            }
        }

        partial void durationMinutes_Validate(EntityValidationResultsBuilder results)
        {
            if (durationMinutes < 0)
            {
                results.AddPropertyError("Duration must be greater than zero.");
            }
        }
    }
}
