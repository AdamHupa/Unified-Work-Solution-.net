/* author: ahupa@polsl.pl, 2018 */

CREATE VIEW Log.vw_sourceaddresses
AS

	SELECT DISTINCT(s.Address) AS Address, COUNT(s.Address) AS Count
		FROM Log.Sources s
		GROUP BY s.Address

-------------------------------------------------------------------------------

	--WITH
	--DistinctFirstEncounter AS
	--(
	--	SELECT DISTINCT (FIRST_VALUE(s.Id) OVER(PARTITION BY s.Address ORDER BY s.Creation)) AS Id
	--	FROM Log.Sources s
	--),
	--Revisits AS
	--(
	--	SELECT DISTINCT(s.Address) AS Address, COUNT(s.Address) AS Count
	--	FROM Log.Sources s
	--	GROUP BY s.Address
	--)
	--SELECT s.Id, s.Address, r.Count, s.Creation
	--FROM Log.Sources AS s
	--JOIN DistinctFirstEncounter AS dfe
	--ON dfe.Id = s.Id
	--JOIN Revisits AS r
	--ON r.Address = s.Address

-------------------------------------------------------------------------------

	--WITH DistinctAddresses AS
	--(
	--	SELECT s.Address, s.Creation, ROW_NUMBER() OVER(PARTITION BY s.Address ORDER BY s.Creation) AS RowNo
	--	FROM Log.Sources s
	--)
	--SELECT *
	--FROM DistinctAddresses
	--WHERE RowNo = 1

-------------------------------------------------------------------------------

GO
