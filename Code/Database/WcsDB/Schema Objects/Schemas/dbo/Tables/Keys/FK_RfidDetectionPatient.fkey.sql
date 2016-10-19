ALTER TABLE [dbo].[RfidDetection]
    ADD CONSTRAINT [FK_RfidDetectionPatient] FOREIGN KEY ([patientId]) REFERENCES [dbo].[Patient] ([patientId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
