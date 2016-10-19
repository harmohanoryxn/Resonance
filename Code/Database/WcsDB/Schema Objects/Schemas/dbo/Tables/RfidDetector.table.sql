CREATE TABLE [dbo].[RfidDetector] (
    [rfidDetectorId] int IDENTITY(1,1) NOT NULL,
	[externalSourceId] int  NOT NULL,
	[externalId] nvarchar(20)  NOT NULL,
	[Location_locationId] int  NULL,
    [WaitingArea_waitingAreaId] int  NULL
	CONSTRAINT UC_RfidDetector_External UNIQUE ([externalSourceId], [externalId])
)
