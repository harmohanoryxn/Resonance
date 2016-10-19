CREATE TABLE [dbo].[AdmissionStatus] (
    [admissionStatusId] INT            IDENTITY (1, 1) NOT NULL,
    [status]            NVARCHAR (20)  NOT NULL,
	CONSTRAINT UC_AdmissionStatus_status UNIQUE ([status])
);

