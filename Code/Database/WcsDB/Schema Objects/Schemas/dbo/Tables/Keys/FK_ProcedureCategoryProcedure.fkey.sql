ALTER TABLE [dbo].[Procedure]
    ADD CONSTRAINT [FK_ProcedureCategoryProcedure] FOREIGN KEY ([ProcedureCategory_procedureCategoryId]) REFERENCES [dbo].[ProcedureCategory] ([procedureCategoryId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

