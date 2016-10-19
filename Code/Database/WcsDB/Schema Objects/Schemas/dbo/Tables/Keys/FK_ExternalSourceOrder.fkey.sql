ALTER TABLE [dbo].[Order]
    ADD CONSTRAINT [FK_ExternalSourceOrder] FOREIGN KEY ([externalSourceId]) REFERENCES [dbo].[ExternalSource] ([externalSourceId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

