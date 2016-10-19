CREATE TABLE [dbo].[Room] (
    [roomId]              INT            IDENTITY (1, 1) NOT NULL,
    [name]                NVARCHAR (50) NOT NULL,
    [Location_locationId] INT            NOT NULL
	CONSTRAINT UC_Room UNIQUE ([name], [Location_locationId])
);

