ALTER TABLE [dbo].[Pin]
    ADD CONSTRAINT [FK_DevicePin] FOREIGN KEY ([Device_deviceId]) REFERENCES [dbo].[Device] ([deviceId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

