ALTER TABLE [dbo].[Configuration]
    ADD CONSTRAINT [FK_ConfigurationConfigurationType] FOREIGN KEY ([ConfigurationType_ConfigurationTypeId]) REFERENCES [dbo].[ConfigurationType] ([ConfigurationTypeId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

