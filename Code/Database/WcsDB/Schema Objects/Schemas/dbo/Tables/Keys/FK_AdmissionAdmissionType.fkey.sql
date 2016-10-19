ALTER TABLE [dbo].[Admission]
    ADD CONSTRAINT [FK_AdmissionAdmissionType] FOREIGN KEY ([AdmissionType_admissionTypeId]) REFERENCES [dbo].[AdmissionType] ([admissionTypeId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
