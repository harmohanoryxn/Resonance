CREATE TABLE [dbo].[Configuration] (
    [configurationId]                       INT           IDENTITY (1, 1) NOT NULL,
    [name]                                  NVARCHAR (50) NOT NULL,
    [ConfigurationType_ConfigurationTypeId] INT           NOT NULL
);

