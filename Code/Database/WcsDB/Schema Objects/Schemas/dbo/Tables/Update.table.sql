CREATE TABLE [dbo].[Update] (
    [updateId]      INT            IDENTITY (1, 1) NOT NULL,
    [type]          NVARCHAR (50)  NULL,
    [source]        NVARCHAR (50)  NULL,
    [value]         NVARCHAR (MAX) NULL,
    [dateCreated]   DATETIME       NULL,
    [Bed_bedId]             INT    NULL,
    [Order_orderId]         INT    NULL,
    [Admission_admissionId] INT    NULL
);

