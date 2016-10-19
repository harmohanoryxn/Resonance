ALTER TABLE [dbo].[RfidDetection]
    ADD CONSTRAINT [FK_RfidDetectionRfidDetector] FOREIGN KEY ([rfidDetectorId]) REFERENCES [dbo].[RfidDetector] ([rfidDetectorId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
