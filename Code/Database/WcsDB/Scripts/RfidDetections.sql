SELECT TOP 1000 
      P.externalId AS Patient
      ,DET.externalId AS DetectorCode
      ,COALESCE(L.[code], WA.[code]) AS LocationWaitingAreaCode
      ,DIR.direction AS Direction
      ,[dateTimeDetected] AS [Timestamp]
FROM [WCS].[dbo].[RfidDetection] RD
  INNER JOIN dbo.RfidDirection DIR ON RD.rfidDirectionId = DIR.rfidDirectionId
  INNER JOIN dbo.Patient P ON RD.patientId = P.patientId
  INNER JOIN dbo.RfidDetector DET ON RD.rfidDetectorId = DET.rfidDetectorId  
  LEFT OUTER JOIN dbo.Location L ON DET.Location_locationId = L.locationId
  LEFT OUTER JOIN dbo.WaitingArea WA ON DET.WaitingArea_waitingAreaId = WA.waitingAreaId
ORDER BY
  [dateTimeDetected] DESC
  