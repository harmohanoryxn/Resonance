CREATE TABLE [dbo].[BedCleaningEventType] (
    [bedCleaningEventTypeId] INT           IDENTITY (1, 1) NOT NULL,
    [eventType]        NVARCHAR (20) NOT NULL
	CONSTRAINT UC_BedCleaningEventType_eventType UNIQUE ([eventType])
);

