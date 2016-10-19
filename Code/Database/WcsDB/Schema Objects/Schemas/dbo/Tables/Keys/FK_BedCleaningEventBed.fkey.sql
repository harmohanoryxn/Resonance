ALTER TABLE [dbo].[BedCleaningEvent]
    ADD CONSTRAINT [FK_BedCleaningEventBed] FOREIGN KEY ([Bed_bedId]) REFERENCES [dbo].[Bed] ([bedId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

