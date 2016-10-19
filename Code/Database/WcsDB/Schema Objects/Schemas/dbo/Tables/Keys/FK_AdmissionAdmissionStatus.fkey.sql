ALTER TABLE [dbo].[Admission]
    ADD CONSTRAINT [FK_AdmissionAdmissionStatus] FOREIGN KEY ([AdmissionStatus_admissionStatusId]) REFERENCES [dbo].[AdmissionStatus] ([admissionStatusId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
