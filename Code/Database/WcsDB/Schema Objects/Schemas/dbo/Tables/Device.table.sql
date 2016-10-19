CREATE TABLE [dbo].[Device] (
    [deviceId]        INT           IDENTITY (1, 1) NOT NULL,
    [name]            NVARCHAR (50) NOT NULL,
    [description]     NVARCHAR (MAX) NOT NULL,
    [os]              NVARCHAR (50) NULL,
    [clientVersion]   NVARCHAR (50) NULL,
    [ipAddress]       NVARCHAR (50) NULL,
    [lastConnection]  DATETIME      NULL,
    [locationId]      INT           NULL,
    [lockTimeout]     INT           NOT NULL,
    [configurationTimeout]     INT  NOT NULL
	CONSTRAINT UC_Device_name UNIQUE ([name])
);

