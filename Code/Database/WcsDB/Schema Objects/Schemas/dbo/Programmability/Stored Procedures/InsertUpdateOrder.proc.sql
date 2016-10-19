CREATE PROCEDURE [dbo].[InsertUpdateOrder]
	@externalSourceId int
	,@externalId nvarchar(50)
	,@orderNumber nvarchar(20)
	,@procedureTime datetime
	,@orderStatusId int
	,@completedTime datetime
	,@admissionId int
	,@clinicalIndicator nvarchar(200)
	,@estimatedProcedureDuration int
	,@Procedure_procedureId int
	,@Department_locationId int
	,@OrderingDoctor_doctorId int
	,@isHidden bit
	,@source nvarchar(50)
	,@history nvarchar(200)
	,@diagnosis nvarchar(200)
	,@currentCardiologist nvarchar(200)
	,@requiresSupervision bit
	,@requiresFootwear bit
	,@requiresMedicalRecords bit
AS

-- Create a temporary table variable to hold the output actions.
DECLARE @mergeOutput TABLE(Change VARCHAR(20), orderId int);

MERGE [Order] AS dest
USING
(
SELECT
	@externalSourceId
	,@externalId
	,@orderNumber
	,@procedureTime
	,@orderStatusId
	,@completedTime
	,@admissionId
	,@clinicalIndicator
	,@estimatedProcedureDuration
	,@Procedure_procedureId
	,@Department_locationId
	,@OrderingDoctor_doctorId
	,@isHidden
	,@history
	,@diagnosis
	,@currentCardiologist
	,@requiresSupervision
	,@requiresFootwear
	,@requiresMedicalRecords

) AS source
(
	[externalSourceId]
	,[externalId]
	,[orderNumber]
	,[procedureTime]
	,[orderStatusId]
	,[completedTime]
	,[admissionId]
	,[clinicalIndicator]
	,[estimatedProcedureDuration]
	,[Procedure_procedureId]
	,[Department_locationId]
	,[OrderingDoctor_doctorId]
	,[isHidden]
	,[history]
	,[diagnosis]
	,[currentCardiologist]
	,[requiresSupervision]
	,[requiresFootwear]
	,[requiresMedicalRecords]
)
ON
(
	dest.externalSourceId = source.externalSourceId
	AND dest.externalId = source.externalId
)
WHEN MATCHED AND
	dest.[orderStatusId] <> @orderStatusId
	OR dest.[completedTime] <> @completedTime
THEN 
UPDATE
	SET
		[orderStatusId] = @orderStatusId
		,[completedTime] = @completedTime
WHEN NOT MATCHED THEN
	INSERT 
		(
			[externalSourceId]
			,[externalId]
			,[orderNumber]
			,[procedureTime]
			,[orderStatusId]
			,[completedTime]
			,[admissionId]
			,[clinicalIndicator]
			,[estimatedProcedureDuration]
			,[Procedure_procedureId]
			,[Department_locationId]
			,[OrderingDoctor_doctorId]
			,[isHidden]
			,[acknowledged]
			,[acknowledgedTime]
			,[acknowledgedBy]
			,[history]
			,[diagnosis]
			,[currentCardiologist]
			,[requiresSupervision]
			,[requiresFootwear]
			,[requiresMedicalRecords]
		)
		VALUES
		(
			@externalSourceId
			,@externalId
			,@orderNumber
			,@procedureTime
			,@orderStatusId
			,@completedTime
			,@admissionId
			,@clinicalIndicator
			,@estimatedProcedureDuration
			,@Procedure_procedureId
			,@Department_locationId
			,@OrderingDoctor_doctorId
			,@isHidden
			,0
			,null
			,null
			,@history
			,@diagnosis
			,@currentCardiologist
			,@requiresSupervision
			,@requiresFootwear
			,@requiresMedicalRecords
		)
OUTPUT $action, inserted.orderId INTO @mergeOutput;

INSERT INTO [Update]
	(
	[type]
	,[source]
	,[value]
	,[dateCreated]
	,[Order_orderId]
	)
SELECT 
	CASE Change
	WHEN 'INSERT' THEN 'Order Imported'
	WHEN 'UPDATE' THEN 'Order Updated'
	END
	,@source
	,@externalId
	,GETDATE()
	,orderId
FROM
	@mergeOutput

RETURN 0