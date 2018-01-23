CREATE VIEW [dbo].[vUnitsOld]
AS
SELECT     UnitID AS id, FullName, ShortName, PPS_Code, FIS_Code, SchoolCode
FROM         Catbert3.dbo.Unit