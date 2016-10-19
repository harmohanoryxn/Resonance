CREATE PROCEDURE [dbo].[InsertUpdateAdmission]
	@externalSourceId int
	,@externalId nvarchar(50)
	,@admitDateTime datetime
	,@dischargeDateTime datetime
	,@patientId int
	,@PrimaryCareDoctor_doctorId int
	,@AttendingDoctor_doctorId int
	,@AdmittingDoctor_doctorId int
	,@AdmissionType_admissionTypeId int
	,@AdmissionStatus_admissionStatusId int
	,@Location_locationId int
	,@Bed_bedId int
	,@source nvarchar(50)
AS

-- Create a temporary table variable to hold the output actions.
DECLARE @mergeOutput TABLE(Change VARCHAR(20), admissionId int);

MERGE [Admission] AS dest
USING
(
SELECT
	@externalSourceId
	,@externalId
	,@admitDateTime
	,@dischargeDateTime
	,@patientId
	,@PrimaryCareDoctor_doctorId
	,@AttendingDoctor_doctorId
	,@AdmittingDoctor_doctorId
	,@AdmissionType_admissionTypeId
	,@AdmissionStatus_admissionStatusId
	,@Location_locationId
	,@Bed_bedId
) AS source
(
	[externalSourceId]
	,[externalId]
	,[admitDateTime]
	,[dischargeDateTime]
	,[patientId]
	,[PrimaryCareDoctor_doctorId]
	,[AttendingDoctor_doctorId]
	,[AdmittingDoctor_doctorId]
	,[AdmissionType_admissionTypeId]
	,[AdmissionStatus_admissionStatusId]
	,[Location_locationId]
	,[Bed_bedId]
)
ON
(
	dest.externalSourceId = source.externalSourceId
	AND dest.externalId = source.externalId
)
WHEN MATCHED AND 
	dest.[admitDateTime] <> @admitDateTime
	OR dest.[dischargeDateTime] <> @dischargeDateTime
	OR dest.[patientId] <> @patientId
	OR dest.[PrimaryCareDoctor_doctorId] <> @PrimaryCareDoctor_doctorId
	OR dest.[AttendingDoctor_doctorId] <> @AttendingDoctor_doctorId
	OR dest.[AdmittingDoctor_doctorId] <> @AdmittingDoctor_doctorId
	OR dest.[AdmissionType_admissionTypeId] <> @AdmissionType_admissionTypeId
	OR dest.[AdmissionStatus_admissionStatusId] <> @AdmissionStatus_admissionStatusId
	OR dest.[Location_locationId] <> @Location_locationId
	OR dest.[Bed_bedId] <> @Bed_bedId
THEN 
UPDATE
	SET
		[admitDateTime] = @admitDateTime
		,[dischargeDateTime] = @dischargeDateTime
		,[patientId] = @patientId
		,[PrimaryCareDoctor_doctorId] = @PrimaryCareDoctor_doctorId
		,[AttendingDoctor_doctorId] = @AttendingDoctor_doctorId
		,[AdmittingDoctor_doctorId] = @AdmittingDoctor_doctorId
		,[AdmissionType_admissionTypeId] = @AdmissionType_admissionTypeId
		,[AdmissionStatus_admissionStatusId] = @AdmissionStatus_admissionStatusId
		,[Location_locationId] = @Location_locationId
		,[Bed_bedId] = @Bed_bedId
WHEN NOT MATCHED THEN	
	INSERT 
		(
		[externalSourceId]
		,[externalId]
		,[admitDateTime]
		,[dischargeDateTime]
		,[patientId]
		,[PrimaryCareDoctor_doctorId]
		,[AttendingDoctor_doctorId]
		,[AdmittingDoctor_doctorId]
		,[AdmissionType_admissionTypeId]
		,[AdmissionStatus_admissionStatusId]
		,[Location_locationId]
		,[Bed_bedId]
		)
     VALUES
		(
		@externalSourceId
		,@externalId
		,@admitDateTime
		,@dischargeDateTime
		,@patientId
		,@PrimaryCareDoctor_doctorId
		,@AttendingDoctor_doctorId
		,@AdmittingDoctor_doctorId
		,@AdmissionType_admissionTypeId
		,@AdmissionStatus_admissionStatusId
		,@Location_locationId
		,@Bed_bedId
		)
OUTPUT $action, inserted.admissionId INTO @mergeOutput;

INSERT INTO [Update]
	(
	[type]
	,[source]
	,[value]
	,[dateCreated]
	,[Bed_bedId]
	,[Admission_admissionId]
	)
SELECT 
	CASE Change
	WHEN 'INSERT' THEN 'Admission Imported'
	WHEN 'UPDATE' THEN 'Admission Updated'
	END
	,@source
	,@externalId
	,GETDATE()
	,@Bed_bedId
	,admissionId
FROM
	@mergeOutput

RETURN 0