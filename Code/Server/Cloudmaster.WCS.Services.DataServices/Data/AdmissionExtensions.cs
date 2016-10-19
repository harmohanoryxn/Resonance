using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WCS.Data.EF;
using WCS.Services.DataServices;

namespace WCS.Services.DataServices.Data
{
	public static class AdmissionExtensions
	{
        public static Admission Convert(this WCS.Data.EF.Admission a, bool isRadiationRisk)
		{
			Admission admission = new Admission();
			admission.AdmissionId = a.admissionId;
            admission.DisplayCode = a.externalId;
			admission.Patient = new Patient
									{
										PatientId = a.Patient.patientId,
										IPeopleNumber = a.Patient.externalId,
										GivenName = a.Patient.givenName,
										FamilyName = a.Patient.surname,
										DateOfBirth = a.Patient.dob,
                                        Sex = a.Patient.sex == "M" ? PatientSex.Male : PatientSex.Female,
										IsAssistanceRequired = a.Patient.isAssistanceRequired,
                                        AssistanceDescription = a.Patient.assistanceDescription
									};

            // TODO: can we make AdmitDateTime & DischargeDateTime nullable like EstimatedDischargeDateTime?
            admission.AdmitDateTime = a.admitDateTime.HasValue?a.admitDateTime.Value:default(DateTime);
            admission.EstimatedDischargeDateTime = a.estimatedDischargeDateTime;
            admission.DischargeDateTime = a.dischargeDateTime.HasValue ? a.dischargeDateTime.Value : default(DateTime);
            admission.Location = new Location();

            if (a.Bed != null)
            {
                admission.Location.Bed = a.Bed.name;
                admission.Location.Room = a.Bed.Room.name;
            }
            
            if(a.Location !=null)
			{
				admission.Location.Name = a.Location.code;
				admission.Location.FullName = a.Location.name;
                admission.Location.IsEmergency = a.Location.isEmergency;
			}

			admission.Status = a.AdmissionStatus.ConvertToAdmissionStatus();
			admission.Type = a.AdmissionType.ConvertToAdmissionType();
			admission.CriticalCareIndicators = new CriticalCareIndicators();
			admission.CriticalCareIndicators.IsMrsaRisk = a.Patient.isMrsaPositive;
			admission.CriticalCareIndicators.IsFallRisk = a.Patient.isFallRisk;
			admission.CriticalCareIndicators.HasLatexAllergy = a.Patient.hasLatexAllergy;
            admission.CriticalCareIndicators.IsRadiationRisk = isRadiationRisk;

            if (a.PrimaryCareDoctor != null)
				admission.PrimaryDoctor = a.PrimaryCareDoctor.name;
			if (a.AdmittingDoctor != null)
				admission.AdmittingDoctor = a.AdmittingDoctor.name;
			if (a.AttendingDoctor != null)
				admission.AttendingDoctor = a.AttendingDoctor.name;

            var updates = a.Updates.ToList();
            var enumerable1 = updates.Select(n => n.Convert());
            admission.Updates = enumerable1.ToList();

			return admission;
		}

		public static AdmissionStatus ConvertToAdmissionStatus(this WCS.Data.EF.AdmissionStatus admissionStatus)
		{
            switch (admissionStatus.status)
            {
                case "Registered":
                    return AdmissionStatus.Registered;
                case "Admitted":
                    return AdmissionStatus.Admitted;
                case "Discharged":
                    return AdmissionStatus.Discharged;
                default:
                    throw new InvalidOperationException("Unknown admission status.");
            }
		}
		 
		public static AdmissionType ConvertToAdmissionType(this WCS.Data.EF.AdmissionType admissionType)
		{
            switch (admissionType.type)
            {
                case "In":
                    return AdmissionType.In;
                case "Out":
                    return AdmissionType.Out;
                case "Day":
                    return AdmissionType.Day;
                default:
                    throw new InvalidOperationException("Unknown admission type.");
            }
		}
	}
}
