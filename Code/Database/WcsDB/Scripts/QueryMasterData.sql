-- ExternalSource sheet
SELECT
  source
FROM
  ExternalSource
ORDER BY
  source ASC

-- Location sheet
SELECT
  code,
  name,
  includeInMerge,
  isEmergency,
  contactInfo
FROM
  Location
ORDER BY
  code ASC

--TODO: add a query for RFID here

-- Bed sheet
SELECT
  L.code AS [location code],
  R.[name] AS [room name],
  B.[name]
FROM
  Bed AS B
  INNER JOIN Room AS R
    ON B.Room_roomId = R.roomId
  INNER JOIN Location AS L
    ON R.Location_locationId = L.locationId
ORDER BY
  L.code, R.[name], B.[name] ASC

-- ProcedureCategory sheet
SELECT
  ES.source AS Source,
  PC.externalId AS externalId,
  PC.includeInMerge AS includeInMerge,
  PC.[description] AS [description]
FROM
  ProcedureCategory AS PC
  INNER JOIN ExternalSource AS ES
    ON PC.externalSourceId = ES.externalSourceId
ORDER BY
  ES.source ASC, PC.externalId ASC

-- Procedure sheet
SELECT
  ES.source AS Source,
  P.externalId AS externalId,
  P.code AS code,
  P.[description] AS [description],
  NR1.[description] AS [notificationRule1 description],
  NR1.[priorToProcedureTime] AS [notificationRule1 priorToProcedureTime],
  NR1.[durationMinutes] AS [notificationRule1 durationMinutes],
  NR1.[isAcknowledgmentRequired] AS [notificationRule1 isAcknowledgmentRequired]
FROM
  [Procedure] AS P
  INNER JOIN ExternalSource AS ES
    ON P.externalSourceId = ES.externalSourceId
  LEFT OUTER JOIN NotificationRule AS NR1
    ON P.procedureId = NR1.Procedure_procedureId
ORDER BY
  ES.source ASC, P.externalId ASC

--TODO: add a query for notification rules here

