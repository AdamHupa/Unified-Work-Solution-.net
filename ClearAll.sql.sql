/* author: ahupa@polsl.pl, 2018 */

/* Note: "SQL80001: Incorrect syntax.."
-- Visual Studio may sometimes generated an incorrect SQL Command error message despite being no syntax errors.
-- Partial correction involves to enable the SQLCMD mode. */


-- functions ------------------------------------------------------------------



-- inline functions -----------------------------------------------------------

IF OBJECT_ID ('Log.fn_select_eventlog', 'IF') IS NOT NULL
	DROP FUNCTION Log.fn_select_eventlog;
GO

IF OBJECT_ID ('Log.fn_select_eventlog_context', 'IF') IS NOT NULL
	DROP FUNCTION Log.fn_select_eventlog_context;
GO

-- procedures -----------------------------------------------------------------

IF OBJECT_ID ('Log.usp_insert_eventlog', 'P') IS NOT NULL
	DROP PROCEDURE Log.usp_insert_eventlog;
GO

IF OBJECT_ID ('Log.usp_insert_eventlog_context', 'P') IS NOT NULL
	DROP PROCEDURE Log.usp_insert_eventlog_context;
GO

-- views ----------------------------------------------------------------------

IF OBJECT_ID ('Log.vw_sourceaddresses', 'V') IS NOT NULL
	DROP VIEW Log.vw_sourceaddresses;
GO

-------------------------------------------------------------------------------
