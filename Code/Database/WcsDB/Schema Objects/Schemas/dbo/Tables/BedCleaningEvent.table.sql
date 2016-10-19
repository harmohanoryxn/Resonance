CREATE TABLE [dbo].[BedCleaningEvent] (
    [bedCleaningEventId] INT      IDENTITY (1, 1) NOT NULL,
    [timestamp]    DATETIME NOT NULL,
    [Bed_bedId]    INT      NOT NULL,
    [bedCleaningEventTypeId] INT NOT NULL
);

