﻿ALTER TABLE [dbo].[Note]
    ADD CONSTRAINT [FK_BedNote] FOREIGN KEY ([bedId]) REFERENCES [dbo].[Bed] ([bedId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
