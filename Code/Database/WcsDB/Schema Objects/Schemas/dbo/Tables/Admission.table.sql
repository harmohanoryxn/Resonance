CREATE TABLE [dbo].[Admission] (
    [admissionId] int IDENTITY(1,1) NOT NULL,
    [externalSourceId] int  NOT NULL,
    [externalId] nvarchar(50)  NOT NULL,
    [admitDateTime] datetime  NULL,
    [estimatedDischargeDateTime] datetime  NULL,
    [dischargeDateTime] datetime  NULL,
    [patientId] int  NOT NULL,
    [PrimaryCareDoctor_doctorId] int  NULL,
    [AttendingDoctor_doctorId] int  NULL,
    [AdmittingDoctor_doctorId] int  NULL,
    [AdmissionType_admissionTypeId] int  NOT NULL,
    [AdmissionStatus_admissionStatusId] int  NOT NULL,
    [Location_locationId] int  NOT NULL,
    [Bed_bedId] int NULL
    CONSTRAINT UC_Admission_External UNIQUE ([externalSourceId], [externalId])
);
