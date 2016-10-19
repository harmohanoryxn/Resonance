ALTER TABLE [dbo].[Admission]
    ADD CONSTRAINT [FK_PatientAdmission] FOREIGN KEY ([patientId]) REFERENCES [dbo].[Patient] ([patientId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

