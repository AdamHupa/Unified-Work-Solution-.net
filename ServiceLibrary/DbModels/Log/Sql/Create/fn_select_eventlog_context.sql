/* author: ahupa@polsl.pl, 2018 */

CREATE FUNCTION Log.fn_select_eventlog_context
(
	@id INT
)
--WITH EXECUTE AS CALLER 
RETURNS TABLE
AS
RETURN
(
	/* Using Common Table Expressions to achieve single query implementation required for
	-- inline functions and to reduces estimated subtree cost comparing to traditional join approach. */

	WITH
	Log_LogRecords AS
	(
		SELECT * FROM Log.LogRecords WHERE Id = @id
	)
	SELECT lr.TimeStamp, lr.Level, lr.Logger,
		   s.Address, s.MachineName, s.WindowsId,
		   cs.CallerSide, cs.LineNumber,
		   c.ThreadId, c.StackTrace,
		   m.Text AS Message,
		   jo_e.Json AS Exception,
		   jo_j.Json AS Json
	FROM Log_LogRecords lr
	LEFT JOIN Log.Sources s
		ON s.Id = lr.SourceId
	LEFT JOIN Log.CallSides cs
		ON cs.Id = lr.CallSideId
	LEFT JOIN Log.Messages m
		ON m.MessageId = lr.MessageId
	LEFT JOIN Log.JsonObjects jo_e
		ON jo_e.JsonId = lr.ExceptionId
	LEFT JOIN Log.JsonObjects jo_j
		ON jo_j.JsonId = lr.JsonId
	LEFT JOIN Log.Contexts c
		ON c.LogId = lr.Id

	-- Note: read about AS block restrictions
)
GO
