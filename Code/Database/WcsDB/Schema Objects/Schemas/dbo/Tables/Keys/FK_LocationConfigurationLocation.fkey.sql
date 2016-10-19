ALTER TABLE [dbo].[ConfigurationLocation]
    ADD CONSTRAINT [FK_LocationConfigurationLocation] FOREIGN KEY ([locationId]) REFERENCES [dbo].[Location] ([locationId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

