ALTER TABLE [dbo].[BedCleaningEvent]
    ADD CONSTRAINT [FK_BedCleaningEventBedCleaningEventType] FOREIGN KEY ([bedCleaningEventTypeId]) REFERENCES [dbo].[BedCleaningEventType] ([bedCleaningEventTypeId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

