CREATE VIEW [dbo].[ApplicationRoles_V]
AS
SELECT dbo.Applications.ApplicationID, dbo.Applications.Name, dbo.Roles.Role, dbo.Roles.Inactive
FROM  dbo.ApplicationRoles INNER JOIN
               dbo.Applications ON dbo.ApplicationRoles.ApplicationID = dbo.Applications.ApplicationID INNER JOIN
               dbo.Roles ON dbo.ApplicationRoles.RoleID = dbo.Roles.RoleID
GO



GO


