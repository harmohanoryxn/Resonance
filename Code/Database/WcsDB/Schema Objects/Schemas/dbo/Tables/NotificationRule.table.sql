CREATE TABLE [dbo].[NotificationRule] (
    [notificationRuleId]    INT         IDENTITY (1, 1) NOT NULL,
    [description]           NVARCHAR (200) NOT NULL,
    [priorToProcedureTime]  INT            NOT NULL,
    [durationMinutes]       INT            NOT NULL,
    [radiationRiskDurationMinutes] INT     NOT NULL,
    [isAcknowledgmentRequired] BIT         NOT NULL,
    [Procedure_procedureId] INT            NOT NULL
);

