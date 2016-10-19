ALTER TABLE [dbo].[AdmissionDoctor]
    ADD CONSTRAINT [FK_DoctorAdmissionDoctor] FOREIGN KEY ([doctorId]) REFERENCES [dbo].[Doctor] ([doctorId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

