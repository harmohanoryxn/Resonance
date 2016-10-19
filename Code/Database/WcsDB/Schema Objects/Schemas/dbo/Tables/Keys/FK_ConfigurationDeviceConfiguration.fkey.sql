ALTER TABLE [dbo].[DeviceConfiguration]
    ADD CONSTRAINT [FK_ConfigurationDeviceConfiguration] FOREIGN KEY ([configurationId]) REFERENCES [dbo].[Configuration] ([configurationId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

