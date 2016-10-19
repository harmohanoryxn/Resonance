CREATE TABLE [dbo].[Connection] (
    [connectionId]   INT      IDENTITY (1, 1) NOT NULL,
    [connectionTime] DATETIME NOT NULL,
    [deviceId]       INT      NOT NULL
);

