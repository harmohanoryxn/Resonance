CREATE TABLE [dbo].[Pin] (
    [pinId]           INT            IDENTITY (1, 1) NOT NULL,
    [pin]             NVARCHAR (20) NOT NULL,
    [Device_deviceId] INT            NOT NULL
);

