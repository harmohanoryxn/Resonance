-- Configuration sheet
SELECT
  C.[name],
  CT.name AS [type]
FROM
  [Configuration] AS C
  INNER JOIN ConfigurationType AS CT
    ON C.ConfigurationType_ConfigurationTypeId = CT.ConfigurationTypeId
ORDER BY
  C.[name] ASC

-- ConfigurationLocation sheet
SELECT
  C.[name] AS [configuration name],
  L.code AS [location code],
  CL.isDefault AS [isDefault]
FROM
  [ConfigurationLocation] AS CL
  INNER JOIN Configuration AS C
    ON CL.configurationId = C.configurationId
  INNER JOIN Location AS L
    ON CL.locationId = L.locationId
ORDER BY
  C.[name] ASC, L.code ASC

-- Device sheet
SELECT
  D.name AS [name],
  L.code AS [location code],
  D.lockTimeout AS [lock timeout],
  D.configurationTimeout AS [configuration timeout],
  P.pin AS [pin]
FROM
  [Device] AS D
  LEFT OUTER JOIN Location AS L
    ON D.locationId = L.locationId
  LEFT JOIN (SELECT Device_deviceId, MIN(pin) AS pin FROM Pin GROUP BY Device_deviceId) AS P
    ON D.deviceId = P.Device_deviceId
ORDER BY
  D.name ASC

-- DeviceConfiguration sheet
SELECT
  D.name AS [name],
  DC.shortcutKeyNo,
  C.[name] AS [configuration],
  DC.cleaningBedDataTimeout,
  DC.orderTimeout,
  DC.presenceTimeout,
  DC.rfidTimeout,
  DC.dischargeTimeout,
  DC.admissionsTimeout
FROM
  [Device] AS D
  INNER JOIN DeviceConfiguration AS DC
    ON D.deviceId = DC.deviceId
  INNER JOIN Configuration AS C
    ON DC.configurationId = C.configurationId
ORDER BY
  D.name ASC, DC.shortcutKeyNo ASC
