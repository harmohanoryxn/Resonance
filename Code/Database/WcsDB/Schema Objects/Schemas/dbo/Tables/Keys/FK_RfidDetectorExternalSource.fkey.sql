ALTER TABLE [dbo].[RfidDetector]
    ADD CONSTRAINT [FK_RfidDetectorExternalSource] FOREIGN KEY ([externalSourceId]) REFERENCES [dbo].[ExternalSource] ([externalSourceId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
