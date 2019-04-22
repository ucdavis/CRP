CREATE VIEW [dbo].[UserUnits_V]
AS
SELECT dbo.Users.UserID, dbo.Users.FirstName, dbo.Users.LastName, dbo.Users.EmployeeID, dbo.Unit.UnitID, dbo.Unit.FullName, dbo.Unit.PPS_Code, 
               dbo.Unit.FIS_Code
FROM  dbo.Unit INNER JOIN
               dbo.UserUnit ON dbo.Unit.UnitID = dbo.UserUnit.UnitID INNER JOIN
               dbo.Users ON dbo.UserUnit.UserID = dbo.Users.UserID
GO
