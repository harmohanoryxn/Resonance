ALTER TABLE [dbo].[DeviceConfiguration]
    ADD CONSTRAINT [FK_DeviceDeviceConfiguration] FOREIGN KEY ([deviceId]) REFERENCES [dbo].[Device] ([deviceId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

