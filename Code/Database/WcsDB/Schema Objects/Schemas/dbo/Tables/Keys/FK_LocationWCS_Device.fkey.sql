ALTER TABLE [dbo].[Device]
    ADD CONSTRAINT [FK_LocationWCS_Device] FOREIGN KEY ([locationId]) REFERENCES [dbo].[Location] ([locationId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

