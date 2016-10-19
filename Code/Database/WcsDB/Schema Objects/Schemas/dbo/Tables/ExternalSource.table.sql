CREATE TABLE [dbo].[ExternalSource] (
    [externalSourceId] INT            IDENTITY (1, 1) NOT NULL,
    [source]        NVARCHAR (20) NOT NULL
    CONSTRAINT UC_ExternalSource_source UNIQUE ([source])
);

