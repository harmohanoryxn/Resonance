using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;

namespace Cloudmaster.WCS.DataServicesInvoker.Types
{
	public static class ClientAdmissionStatusExtensions
	{
        public static MultiSelectAdmissionTypeFlag ToMultiSelectAdmissionTypeFlag(this AdmissionType admissionType)
		{
			switch (admissionType)
			{
			    case AdmissionType.In:
			        return MultiSelectAdmissionTypeFlag.In;

                case AdmissionType.Out:
                    return MultiSelectAdmissionTypeFlag.Out;

                case AdmissionType.Day:
                    return MultiSelectAdmissionTypeFlag.Day;

                default:
                    return MultiSelectAdmissionTypeFlag.Unknown;
			}
		}

        public static bool IsSet(this MultiSelectAdmissionTypeFlag flags, MultiSelectAdmissionTypeFlag flagsToTest)
        {
            return Enum.GetValues(flagsToTest.GetType()).Cast<Enum>().Any(value => (!value.Equals(MultiSelectAdmissionTypeFlag.Unknown)) && flagsToTest.HasFlag(value) && flags.HasFlag(value));
        }
	}
}
