ALTER TABLE [dbo].[AdmissionDoctor]
    ADD CONSTRAINT [FK_AdmissionDoctorDoctorType] FOREIGN KEY ([DoctorType_doctorTypeId]) REFERENCES [dbo].[DoctorType] ([doctorTypeId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

