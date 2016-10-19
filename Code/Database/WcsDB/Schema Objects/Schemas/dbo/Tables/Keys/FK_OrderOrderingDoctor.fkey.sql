ALTER TABLE [dbo].[Order]
    ADD CONSTRAINT [FK_OrderOrderingDoctor] FOREIGN KEY ([OrderingDoctor_doctorId]) REFERENCES [dbo].[Doctor] ([doctorId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

