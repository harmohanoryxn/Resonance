ALTER TABLE [dbo].[ConfigurationLocation]
    ADD CONSTRAINT [FK_ConfigurationConfigurationLocation] FOREIGN KEY ([configurationId]) REFERENCES [dbo].[Configuration] ([configurationId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

