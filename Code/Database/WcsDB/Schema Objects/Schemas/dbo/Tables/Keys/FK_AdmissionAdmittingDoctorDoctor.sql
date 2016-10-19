ALTER TABLE [dbo].[Admission]
    ADD CONSTRAINT [FK_AdmissionAdmittingDoctorDoctor] FOREIGN KEY ([AdmittingDoctor_doctorId]) REFERENCES [dbo].[Doctor] ([doctorId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
