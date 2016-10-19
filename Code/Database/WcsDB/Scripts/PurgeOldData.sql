-- Purges old connection and log records that are no longer needed and only add to database size
-- Uses chunking to prevent transaction log growing too much
SET NOCOUNT ON

DECLARE @chunkSize INT
SELECT @chunkSize = 10000

DECLARE @countBefore INT
DECLARE @countAfter INT
DECLARE @numRecordsToDelete INT
DECLARE @numChunksDeleted INT
DECLARE @numChunksToDelete INT

-- Delete old connection records in chunks
PRINT 'Deleting connection records...'
SELECT @countBefore = COUNT(*) FROM [Connection]
PRINT CAST(@countBefore AS NVARCHAR(20)) + ' records in [Connection] before deletion'

SELECT @numRecordsToDelete = COUNT(*) FROM [Connection] WHERE [connectionTime] < getdate() - 7
PRINT CAST(@numRecordsToDelete AS NVARCHAR(20)) + ' records in [Connection] for deletion'

SET @numChunksToDelete = @numRecordsToDelete / @chunkSize
SET @numChunksDeleted = 0

WHILE (@numChunksDeleted < @numChunksToDelete)
  BEGIN
  SET @numChunksDeleted = @numChunksDeleted + 1
  BEGIN TRAN
  PRINT 'Deleting chunk ' + CAST(@numChunksDeleted AS NVARCHAR(20)) + ' of ' + CAST(@numChunksToDelete AS NVARCHAR(20)) + '...'
  DELETE TOP(@chunkSize) FROM [Connection] where [connectionTime] < getdate() - 7
  COMMIT TRAN
END

SELECT @countAfter = COUNT(*) FROM [Connection]
PRINT CAST(@countAfter AS NVARCHAR(20)) + ' records in [Connection] after deletion'

PRINT ''

-- Delete old Log records in chunks
PRINT 'Deleting log records...'
SELECT @countBefore = COUNT(*) FROM [Log]
PRINT CAST(@countBefore AS NVARCHAR(20)) + ' records in [Log] before deletion'

SELECT @numRecordsToDelete = COUNT(*) FROM [Log] WHERE [date] < getdate() - 7
PRINT CAST(@numRecordsToDelete AS NVARCHAR(20)) + ' records in [Log] for deletion'

SET @numChunksToDelete = @numRecordsToDelete / @chunkSize
SET @numChunksDeleted = 0

WHILE (@numChunksDeleted < @numChunksToDelete)
  BEGIN
  SET @numChunksDeleted = @numChunksDeleted + 1
  BEGIN TRAN
  PRINT 'Deleting chunk ' + CAST(@numChunksDeleted AS NVARCHAR(20)) + ' of ' + CAST(@numChunksToDelete AS NVARCHAR(20)) + '...'
  DELETE TOP(@chunkSize) FROM [Log] where [date] < getdate() - 7
  COMMIT TRAN
END

SELECT @countAfter = COUNT(*) FROM [Log]
PRINT CAST(@countAfter AS NVARCHAR(20)) + ' records in [Log] after deletion'

SET NOCOUNT OFF