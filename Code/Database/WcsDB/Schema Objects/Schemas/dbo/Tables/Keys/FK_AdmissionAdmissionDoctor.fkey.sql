ALTER TABLE [dbo].[AdmissionDoctor]
    ADD CONSTRAINT [FK_AdmissionAdmissionDoctor] FOREIGN KEY ([admissionId]) REFERENCES [dbo].[Admission] ([admissionId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

