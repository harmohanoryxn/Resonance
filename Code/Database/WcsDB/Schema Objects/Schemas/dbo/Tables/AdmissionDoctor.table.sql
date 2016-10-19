CREATE TABLE [dbo].[AdmissionDoctor] (
    [admissionDoctorId]       INT IDENTITY (1, 1) NOT NULL,
    [admissionId]             INT NOT NULL,
    [doctorId]                INT NOT NULL,
    [DoctorType_doctorTypeId] INT NOT NULL
);

