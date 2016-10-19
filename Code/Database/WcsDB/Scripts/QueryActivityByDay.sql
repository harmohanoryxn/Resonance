SELECT
	*
FROM
(
	SELECT 
		DATEADD(dd,(DATEDIFF(dd,0,[dateCreated])),0) AS [Date],
		[type],
		COUNT(*) AS [Count]
	  FROM [WCS].[dbo].[Update]
	GROUP BY
	  DATEADD(dd,(DATEDIFF(dd,0,[dateCreated])),0), [type]
) AS UNSORTED
ORDER BY
  [Date] DESC
