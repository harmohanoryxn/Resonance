ALTER TABLE [dbo].[Admission]
    ADD CONSTRAINT [FK_ExternalSourceAdmission] FOREIGN KEY ([externalSourceId]) REFERENCES [dbo].[ExternalSource] ([externalSourceId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

