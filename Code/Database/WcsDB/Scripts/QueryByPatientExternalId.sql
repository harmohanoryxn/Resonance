DECLARE @patientExternalId nvarchar(50)
SET @patientExternalId = 'MR00003807'

SELECT TOP 1000 [patientId]
      ,[externalSourceId]
      ,[externalId]
      ,[givenName]
      ,[surname]
      ,[dob]
      ,[sex]
      ,[isMrsaPositive]
      ,[isFallRisk]
      ,[isAssistanceRequired]
      ,[assistanceDescription]
      ,[hasLatexAllergy]
  FROM [Patient]
  where externalId = @patientExternalId

DECLARE @patientId int
SELECT @patientId = [patientId] FROM [Patient] where externalId = @patientExternalId

SELECT TOP 1000
      A.[admissionId]
      ,A.[externalSourceId]
      ,A.[externalId]
      ,L.[code] AS LocationCode
      ,L.[name] AS LocationName
      ,A.[admitDateTime]
      ,A.[estimatedDischargeDateTime]
      ,A.[dischargeDateTime]
      ,A.[patientId]
      ,A.[PrimaryCareDoctor_doctorId]
      ,A.[AttendingDoctor_doctorId]
      ,A.[AdmittingDoctor_doctorId]
      ,A.[AdmissionType_admissionTypeId]
      ,A.[AdmissionStatus_admissionStatusId]
      ,A.[Bed_bedId]
  FROM
  [Admission] AS A
    INNER JOIN Location AS L ON A.Location_locationId = L.locationId
  WHERE patientId = @patientId
  ORDER BY [admissionId] DESC

DECLARE @admissionId int
SET @admissionId = (SELECT TOP 1 [admissionId] FROM [Admission] WHERE patientId = @patientId ORDER BY [admissionId] DESC)

SELECT TOP 1000 O.[orderId]
      ,O.[externalSourceId]
      ,O.[externalId]
      ,O.[orderNumber]
      ,L.[code] AS LocationCode
      ,L.[name] AS LocationName
      ,O.[procedureTime]
      ,O.[orderStatusId]
      ,O.[completedTime]
      ,O.[admissionId]
      ,O.[clinicalIndicator]
      ,O.[estimatedProcedureDuration]
      ,O.[Procedure_procedureId]
      ,O.[OrderingDoctor_doctorId]
      ,O.[isHidden]
      ,O.[acknowledged]
      ,O.[acknowledgedTime]
      ,O.[acknowledgedBy]
  FROM
    [Order] AS O
    INNER JOIN Location AS L ON O.Department_locationId = L.locationId
  WHERE [admissionId] = @admissionId
  ORDER BY [orderId] DESC

DECLARE @orderId int
SET @orderId = (SELECT TOP 1 [orderId] FROM [Order] WHERE [admissionId] = @admissionId ORDER BY [orderId] DESC)

SELECT TOP 1000 [updateId]
      ,[type]
      ,[source]
      ,[value]
      ,[dateCreated]
      ,[Bed_bedId]
      ,[Order_orderId]
      ,[Admission_admissionId]
  FROM [Update]
  where Order_orderId = @orderId