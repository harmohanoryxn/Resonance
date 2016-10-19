ALTER TABLE [dbo].[Admission]
    ADD CONSTRAINT [FK_AdmissionAttendingDoctorDoctor] FOREIGN KEY ([AttendingDoctor_doctorId]) REFERENCES [dbo].[Doctor] ([doctorId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
