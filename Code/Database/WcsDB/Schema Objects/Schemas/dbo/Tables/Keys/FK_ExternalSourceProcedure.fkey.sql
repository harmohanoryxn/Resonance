ALTER TABLE [dbo].[Procedure]
    ADD CONSTRAINT [FK_ExternalSourceProcedure] FOREIGN KEY ([externalSourceId]) REFERENCES [dbo].[ExternalSource] ([externalSourceId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

