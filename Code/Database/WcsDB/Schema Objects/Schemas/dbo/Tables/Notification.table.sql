CREATE TABLE [dbo].[Notification] (
    [notificationId]       INT         IDENTITY (1, 1) NOT NULL,
    [notificationTypeId]  INT             NOT NULL,
    [description]          NVARCHAR (200) NOT NULL,
    [priorToProcedureTime] INT            NOT NULL,
    [isAcknowledgmentRequired]       BIT  NOT NULL,
    [acknowledged]         BIT            NOT NULL,
    [acknowledgedTime]     DATETIME       NULL,
    [acknowledgedBy]       NVARCHAR (20)  NULL,
    [notificationOrder]    INT            NULL,
    [orderId]              INT            NOT NULL,
    [durationMinutes]      INT            NOT NULL,
    [radiationRiskDurationMinutes] INT    NOT NULL
);

