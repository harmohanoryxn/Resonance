ALTER TABLE [dbo].[Patient]
    ADD CONSTRAINT [FK_PatientExternalSource] FOREIGN KEY ([externalSourceId]) REFERENCES [dbo].[ExternalSource] ([externalSourceId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
