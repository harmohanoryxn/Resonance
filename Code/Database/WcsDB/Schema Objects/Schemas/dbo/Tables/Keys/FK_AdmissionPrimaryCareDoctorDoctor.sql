ALTER TABLE [dbo].[Admission]
    ADD CONSTRAINT [FK_AdmissionPrimaryCareDoctor] FOREIGN KEY ([PrimaryCareDoctor_doctorId]) REFERENCES [dbo].[Doctor] ([doctorId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
