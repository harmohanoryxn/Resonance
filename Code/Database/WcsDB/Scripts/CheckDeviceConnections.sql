SELECT TOP 1000 [deviceId]
      ,d.[name]
      ,[os]
      ,[clientVersion]
      ,[description]
      ,[ipAddress]
      ,[lastConnection]
      ,l.[name]
FROM [WCS].[dbo].[Device] D
  LEFT OUTER JOIN location l ON l.[locationId] = d.[locationId]
ORDER BY lastconnection DESC