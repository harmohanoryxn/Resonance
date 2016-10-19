ALTER TABLE [dbo].[ProcedureCategory]
    ADD CONSTRAINT [FK_ExternalSourceProcedureCategory] FOREIGN KEY ([externalSourceId]) REFERENCES [dbo].[ExternalSource] ([externalSourceId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

