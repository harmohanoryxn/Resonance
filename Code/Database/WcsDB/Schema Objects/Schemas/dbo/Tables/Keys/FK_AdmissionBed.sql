﻿ALTER TABLE [dbo].[Admission]
    ADD CONSTRAINT [FK_AdmissionBed] FOREIGN KEY ([Bed_bedId]) REFERENCES [dbo].[Bed] ([bedId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
