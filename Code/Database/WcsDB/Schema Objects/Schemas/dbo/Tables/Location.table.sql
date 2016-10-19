CREATE TABLE [dbo].[Location] (
    [locationId]  INT           IDENTITY (1, 1) NOT NULL,
    [code]        NVARCHAR (20) NOT NULL,
    [name]        NVARCHAR (50) NOT NULL,
    [isEmergency] BIT NOT NULL,
    [contactInfo] NVARCHAR (20)  NULL,
    [includeInMerge] BIT NOT NULL,
    [WaitingArea_waitingAreaId] int  NULL
	CONSTRAINT UC_Location_code UNIQUE ([code])
);

