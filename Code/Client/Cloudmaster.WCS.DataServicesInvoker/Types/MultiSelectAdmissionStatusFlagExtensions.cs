using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cloudmaster.WCS.DataServicesInvoker.Types
{
    public static class MultiSelectAdmissionStatusExtensions
	{
		public static MultiSelectAdmissionStatusFlag Toggle(this MultiSelectAdmissionStatusFlag flags, MultiSelectAdmissionStatusFlag flagsToToggle)
		{
			if (!flags.IsSet(flagsToToggle))
				return flags.Set(flagsToToggle);
			else
				return flags.Clear(flagsToToggle);
		}
		public static bool IsSet(this MultiSelectAdmissionStatusFlag flags, MultiSelectAdmissionStatusFlag flagsToTest)
		{
            return Enum.GetValues(flagsToTest.GetType()).Cast<Enum>().Any(value => (!value.Equals(MultiSelectAdmissionStatusFlag.Unknown)) && flagsToTest.HasFlag(value) && flags.HasFlag(value));
		}
		public static MultiSelectAdmissionStatusFlag Set(this MultiSelectAdmissionStatusFlag flags, MultiSelectAdmissionStatusFlag flagsToSet)
		{
			return flags | flagsToSet;
		}
		public static MultiSelectAdmissionStatusFlag Clear(this MultiSelectAdmissionStatusFlag flags, MultiSelectAdmissionStatusFlag flagsToClear)
		{
			return flags & ~flagsToClear;
		}
	}
}
