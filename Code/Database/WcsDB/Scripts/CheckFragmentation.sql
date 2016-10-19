DECLARE @db_id int; 
SET @db_id = DB_ID(); 
SELECT QUOTENAME(o.name), o.object_id, index_id, avg_fragmentation_in_percent, page_count 
FROM sys.dm_db_index_physical_stats(@db_id, NULL, NULL, NULL, NULL) i
join sys.objects AS o on o.object_id = i.object_id
where avg_fragmentation_in_percent > 10
