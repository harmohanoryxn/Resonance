ALTER TABLE [dbo].[Order]
    ADD CONSTRAINT [FK_OrderLocation] FOREIGN KEY ([Department_locationId]) REFERENCES [dbo].[Location] ([locationId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

