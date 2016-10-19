ALTER TABLE [dbo].[Connection]
    ADD CONSTRAINT [FK_DeviceConnection] FOREIGN KEY ([deviceId]) REFERENCES [dbo].[Device] ([deviceId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

