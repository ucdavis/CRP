CREATE VIEW [dbo].[ApplicationUserUnits_V]
AS
SELECT        dbo.Applications.ApplicationID, dbo.Applications.Name, dbo.Users.UserID, dbo.Users.FirstName, dbo.Users.LastName, dbo.Users.EmployeeID, dbo.Unit.UnitID, 
                         dbo.Unit.FullName, dbo.Unit.PPS_Code, dbo.Unit.FIS_Code, dbo.UnitAssociations.Inactive
FROM            dbo.Unit INNER JOIN
                         dbo.UnitAssociations ON dbo.Unit.UnitID = dbo.UnitAssociations.UnitID INNER JOIN
                         dbo.Users ON dbo.UnitAssociations.UserID = dbo.Users.UserID INNER JOIN
                         dbo.Applications ON dbo.UnitAssociations.ApplicationID = dbo.Applications.ApplicationID
GO



GO



GO


