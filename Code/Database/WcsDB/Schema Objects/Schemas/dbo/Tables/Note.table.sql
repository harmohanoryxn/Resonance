CREATE TABLE [dbo].[Note] (
    [noteId]      INT            IDENTITY (1, 1) NOT NULL,
    [source]      NVARCHAR (50)  NULL,
    [notes]       NVARCHAR (MAX) NULL,
    [dateCreated] DATETIME       NULL,
    [noteOrder]   INT            NOT NULL,
    [bedId]       INT            NULL,
    [orderId]     INT            NULL
);

