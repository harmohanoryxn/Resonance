CREATE TABLE [dbo].[AdmissionType] (
    [admissionTypeId] INT            IDENTITY (1, 1) NOT NULL,
    [type]            NVARCHAR (20)  NOT NULL,
	CONSTRAINT UC_AdmissionType_type UNIQUE ([type])
);

