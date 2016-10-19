ALTER TABLE [dbo].[Admission]
    ADD CONSTRAINT [FK_AdmissionLocation] FOREIGN KEY ([Location_locationId]) REFERENCES [dbo].[Location] ([locationId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

