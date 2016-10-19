ALTER TABLE [dbo].[NotificationRule]
    ADD CONSTRAINT [FK_NotificationRuleProcedure] FOREIGN KEY ([Procedure_procedureId]) REFERENCES [dbo].[Procedure] ([procedureId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

