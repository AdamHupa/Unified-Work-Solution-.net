/* author: ahupa@polsl.pl, 2018 */

CREATE PROCEDURE Log.usp_insert_eventlog
(
	@time_stamp DATETIME2(7), @level TINYINT, @logger NVARCHAR(64),
	@address NVARCHAR(80), @machine_name NVARCHAR(50), @windows_id NVARCHAR(50),
	@call_side NVARCHAR(128), @line_number INT,
	@message NVARCHAR(256) = NULL, @exception NVARCHAR(MAX) = NULL, @json_object NVARCHAR(MAX) = NULL
)
--WITH EXECUTE AS OWNER, SCHEMABINDING, NATIVE_COMPILATION
AS
BEGIN --ATOMIC WITH (TRANSACTION ISOLATION LEVEL = SNAPSHOT, LANGUAGE = N'us_english')

	/* Required to prevent NLog from adding phantom empty string to the log database.
	-- null, empty, whitespace:      ISNULL(LTRIM(RTRIM(col)),'')=''
	-- not null, empty, whitespaces: ISNULL(LTRIM(RTRIM(col)),'')<>'' */

	IF ISNULL(LTRIM(RTRIM(@logger)),'')=''
		--THROW 50000, 'Error: Inconsistent (Level) parameter value entered for Log.LogRecords table.', 1
		--RAISERROR (N'Error: Inconsistent (%s) parameter value entered for %s table.', 16, 1, N'Level', N'Log.EventLogs')
		RETURN


	DECLARE @source_id int;
	DECLARE @call_side_id int;
	DECLARE @message_id int;
	DECLARE @exception_id int;
	DECLARE @json_id int;

	/*DECLARE @source_hash NVARCHAR(32);
	--SET @source_hash =  HashBytes ('MD5', CONCAT(@Address, @machine_name, @windows_id)); */

	/* get or insert logic - operations are optimized for cases where values are already in table */

	SELECT @source_id = Id
	FROM Log.Sources
	WHERE Address = @address AND MachineName = @machine_name AND WindowsId = @windows_id

	IF @source_id IS NULL
	BEGIN
		INSERT INTO Log.Sources (Address, MachineName, WindowsId)
		VALUES (@address, @machine_name, @windows_id);

		IF @@ROWCOUNT = 0
			SELECT @source_id = Id
			FROM Log.Sources
			WHERE Address = @address AND MachineName = @machine_name AND WindowsId = @windows_id
		ELSE
			SET @source_id = SCOPE_IDENTITY()
	END;


	SELECT @call_side_id = Id
	FROM Log.CallSides
	WHERE CallerSide = @call_side AND LineNumber = @line_number;

	IF @call_side_id IS NULL
	BEGIN
		INSERT INTO Log.CallSides (CallerSide, LineNumber)
		VALUES (@call_side, @line_number);

		IF @@ROWCOUNT = 0
			SELECT @call_side_id = Id
			FROM Log.CallSides
			WHERE CallerSide = @call_side AND LineNumber = @line_number;
		ELSE
			SET @call_side_id = SCOPE_IDENTITY()
	END;

	/* always insert */

	IF @message IS NOT NULL
	BEGIN
		INSERT INTO Log.Messages (Text)
		VALUES (@message)
		SET @message_id = SCOPE_IDENTITY()
	END;

	--IF @exception IS NOT NULL
	IF ISNULL(LTRIM(RTRIM(@exception)),'')<>''
	BEGIN
		INSERT INTO Log.JsonObjects (Json)
		VALUES (@exception)
		SET @exception_id = SCOPE_IDENTITY()
	END;

	--IF @json_object IS NOT NULL
	IF ISNULL(LTRIM(RTRIM(@json_object)),'')<>''
	BEGIN
		INSERT INTO Log.JsonObjects (Json)
		VALUES (@json_object)
		SET @json_id = SCOPE_IDENTITY()
	END;

	/* insert to EventLogs */

	INSERT INTO Log.LogRecords VALUES (@time_stamp, @level, @logger, @source_id, @call_side_id, @message_id, @exception_id, @json_id);

END;
GO
