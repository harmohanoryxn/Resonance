ALTER TABLE [dbo].[RfidDetection]
    ADD CONSTRAINT [FK_RfidDetectionRfidDirection] FOREIGN KEY ([rfidDirectionId]) REFERENCES [dbo].[RfidDirection] ([rfidDirectionId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
