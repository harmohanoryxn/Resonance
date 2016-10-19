ALTER TABLE [dbo].[Room]
    ADD CONSTRAINT [FK_LocationRoom] FOREIGN KEY ([Location_locationId]) REFERENCES [dbo].[Location] ([locationId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

