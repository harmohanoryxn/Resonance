CREATE TABLE [dbo].[Bed] (
    [bedId]       INT           IDENTITY (1, 1) NOT NULL,
    [name]        NVARCHAR (50) NOT NULL,
    [Room_roomId] INT           NULL
);

