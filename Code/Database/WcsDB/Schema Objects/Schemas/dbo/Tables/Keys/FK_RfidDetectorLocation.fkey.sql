ALTER TABLE [dbo].[RfidDetector]
    ADD CONSTRAINT [FK_RfidDetectorLocation] FOREIGN KEY ([Location_locationId]) REFERENCES [dbo].[Location] ([locationId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
