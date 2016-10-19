CREATE TRIGGER trgMakeOrderPrepNotification ON dbo.[Order] 
FOR INSERT 
AS
INSERT INTO [dbo].[Notification]
           ([description]
		   ,[notificationTypeId]
           ,[priorToProcedureTime]
           ,[isAcknowledgmentRequired]
           ,[acknowledged]
           ,[acknowledgedTime]
           ,[acknowledgedBy]
           ,[notificationOrder]
           ,[orderId]
           ,[durationMinutes]
		   ,[radiationRiskDurationMinutes])
SELECT
	NR.[description]
	, ( SELECT notificationTypeId FROM [NotificationType] WHERE [name] = 'Prep' )
	, priorToProcedureTime
	, isAcknowledgmentRequired
	, 0
	, null
	, null
	, null
	, O.orderid
	, NR.durationMinutes
	, NR.radiationRiskDurationMinutes
FROM notificationRule NR
INNER JOIN [order] O ON O.Procedure_procedureId = NR.Procedure_procedureId
INNER JOIN inserted i ON i.orderid = o.orderid
