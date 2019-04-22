
CREATE VIEW [dbo].[vUnits]
AS
SELECT     UnitID AS id, FullName, ShortName, PPS_Code, FIS_Code, SchoolCode
FROM         Unit
GO
