using System;
using System.Collections.Generic;


namespace Woodpecker.Core.Sql
{
    public class AzureSqlDmvResourcePecker : AzureSqlDmvPeckerBase
    {
        private const string DatabaseResourceInlineSqlNotSoBad = @"
WITH a AS (
	SELECT		
		MAX(avg_cpu_percent) as avg_cpu_prc,
		MAX(avg_data_io_percent) as avg_data_io_prc,
		MAX(avg_log_write_percent) as avg_log_write_prc,
		MAX(avg_memory_usage_percent) as avg_memory_prc,
		MAX(xtp_storage_percent) as xtp_storage_prc,
		MAX(max_worker_percent) as max_worker_prc,
		MAX(max_session_percent) as max_session_prc,
		(SELECT MAX(v) FROM (VALUES (MAX(avg_cpu_percent)), (MAX(avg_data_io_percent)), (MAX(avg_log_write_percent)), (MAX(max_worker_percent)), (MAX(max_session_percent))) AS value(v)) AS max_dtu_prc,
		MAX(dtu_limit) as  dtu_limit,   
		CAST(DATABASEPROPERTYEX(DB_NAME(),'MaxSizeInBytes') AS FLOAT) AS max_db_size_bytes,
		(SELECT SUM(ps.reserved_page_count) * 8 * 1024 FROM sys.dm_db_partition_stats ps) AS db_bytes_used,
		(SELECT COUNT([program_name]) FROM sys.dm_exec_sessions) AS session_count,
		(SELECT COUNT(*) FROM sys.dm_exec_requests WHERE blocking_session_id <> 0) AS blocking_count
	FROM sys.dm_db_resource_stats
	WHERE end_time > DATEADD(MINUTE,-1,GETUTCDATE())
)

SELECT
	GETUTCDATE() AS collection_time_utc, -- datetime
	@@SERVERNAME AS server_name, -- string
	DB_NAME() AS database_name, -- string
	CONCAT(@@SERVERNAME,'/',DB_NAME()) AS [full_name], -- string
	avg_cpu_prc AS sqldb_resources_cpu_prc, -- float
	avg_data_io_prc AS sqldb_resources_dataio_prc, -- float
	avg_log_write_prc AS sqldb_resources_logwrite_prc, -- float
	avg_memory_prc AS sqldb_resources_memory_prc, -- float
	xtp_storage_prc AS sqldb_resources_xtp_prc, -- float
	max_worker_prc AS sqldb_resources_worker_prc, -- float
	max_session_prc AS sqldb_resources_session_prc, -- float
	max_dtu_prc AS sqldb_resources_dtu_prc, -- float
	dtu_limit AS sqldb_resources_dtu_limit, -- in32
	max_db_size_bytes AS sqldb_resources_maxdbsize_bytes, -- in64
	db_bytes_used AS sqldb_resources_dbused_bytes, -- in64
	session_count AS sqldb_resources_session_count, -- int32
	blocking_count AS sqldb_resources_blocking_count,-- int32
	master_io AS sqldb_resources_masterdbio, -- in64
	master_io_stall_ms AS sqldb_resources_masterdbiostall_ms, -- in64
	tempdb_io AS sqldb_resources_tempdbio, -- in64
	tempdb_io_stall_ms AS sqldb_resources_tempdbiostall_ms, -- in64
	model_io AS sqldb_resources_modeldbio, -- in64
	model_io_stall_ms AS sqldb_resources_modeldbiostall_ms, -- in64
	userdb_io AS sqldb_resources_userdbio, -- in64
	userdb_io_stall_ms AS sqldb_resources_userdbiostall_ms -- in64
FROM a
CROSS APPLY (
	SELECT
		SUM(CASE database_id WHEN 1 THEN num_of_reads + num_of_writes ELSE NULL END) AS master_io,
		SUM(CASE database_id WHEN 1 THEN io_stall ELSE NULL END) AS master_io_stall_ms,
		SUM(CASE database_id WHEN 2 THEN num_of_reads + num_of_writes ELSE NULL END) AS tempdb_io,
		SUM(CASE database_id WHEN 2 THEN io_stall ELSE NULL END) AS tempdb_io_stall_ms,
		SUM(CASE database_id WHEN 3 THEN num_of_reads + num_of_writes ELSE NULL END) AS model_io,
		SUM(CASE database_id WHEN 3 THEN io_stall ELSE NULL END) AS model_io_stall_ms,
		SUM(CASE WHEN database_id > 4 THEN num_of_reads + num_of_writes ELSE NULL END) AS userdb_io,
		SUM(CASE WHEN database_id > 4 THEN io_stall ELSE NULL END) AS userdb_io_stall_ms
	FROM sys.dm_io_virtual_file_stats(NULL,NULL)
	WHERE database_id <> 4
) io"; // TODO: In the future we might need to pass the variable to DATEADD


        protected override string GetQuery()
        {
            return DatabaseResourceInlineSqlNotSoBad;
        }

        protected override IEnumerable<string> GetRowKeyFieldNames()
        {
            return new[] {"resource", "server_name", "database_name"};
        }

        protected override string GetUtcTimestampFieldName()
        {
            return "collection_time_utc";
        }
    }
}
