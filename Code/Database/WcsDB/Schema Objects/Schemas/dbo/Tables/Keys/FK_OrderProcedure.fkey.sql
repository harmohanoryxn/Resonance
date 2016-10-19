ALTER TABLE [dbo].[Order]
    ADD CONSTRAINT [FK_OrderProcedure] FOREIGN KEY ([Procedure_procedureId]) REFERENCES [dbo].[Procedure] ([procedureId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

