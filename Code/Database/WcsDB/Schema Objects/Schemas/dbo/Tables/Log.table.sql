CREATE TABLE [dbo].[Log] (
    [logId]     INT            IDENTITY (1, 1) NOT NULL,
    [Date]      DATETIME       NOT NULL,
    [ComputerName]    NVARCHAR (50)  NOT NULL,
    [Thread]    NVARCHAR (50)  NOT NULL,
    [Level]     NVARCHAR (50)   NOT NULL,
    [Logger]    NVARCHAR (50)  NOT NULL,
    [Message]   NVARCHAR (MAX)  NOT NULL,
    [Exception] NVARCHAR (MAX) NULL
);

