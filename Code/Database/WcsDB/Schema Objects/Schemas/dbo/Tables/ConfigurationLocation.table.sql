CREATE TABLE [dbo].[ConfigurationLocation] (
    [configurationLocationId] INT IDENTITY (1, 1) NOT NULL,
    [configurationId]         INT NOT NULL,
    [locationId]              INT NOT NULL,
    [isDefault]               BIT NOT NULL
);

