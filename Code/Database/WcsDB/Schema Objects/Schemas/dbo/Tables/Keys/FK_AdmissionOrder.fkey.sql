ALTER TABLE [dbo].[Order]
    ADD CONSTRAINT [FK_AdmissionOrder] FOREIGN KEY ([admissionId]) REFERENCES [dbo].[Admission] ([admissionId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

