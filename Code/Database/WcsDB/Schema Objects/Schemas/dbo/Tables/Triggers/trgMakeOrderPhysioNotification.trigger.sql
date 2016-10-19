CREATE TRIGGER trgMakeOrderPhysioNotification ON dbo.[Order] 
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
	P.assistanceDescription
	, ( SELECT notificationTypeId FROM [NotificationType] WHERE [name] = 'Physio' )
	, 15
	, 1
	, 0
	, null
	, null
	, null
	, O.orderid
	, 15
	, 0
FROM
Inserted I
INNER JOIN [Order] O ON I.orderid = O.orderid
INNER JOIN [Admission] A ON O.admissionId = A.admissionId
INNER JOIN [Patient] P ON A.patientId = P.patientId
WHERE P.isAssistanceRequired = 1