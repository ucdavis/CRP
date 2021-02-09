CREATE VIEW [dbo].[ApplicationUsers_V]
AS
SELECT dbo.Applications.ApplicationID, dbo.Applications.Name, dbo.Users.UserID, dbo.Users.FirstName, dbo.Users.LastName, dbo.Users.EmployeeID, dbo.Roles.Role, 
               dbo.Permissions.Inactive, dbo.Permissions.PermissionID
FROM  dbo.Applications INNER JOIN
               dbo.Permissions ON dbo.Applications.ApplicationID = dbo.Permissions.ApplicationID INNER JOIN
               dbo.Users ON dbo.Permissions.UserID = dbo.Users.UserID INNER JOIN
               dbo.Roles ON dbo.Permissions.RoleID = dbo.Roles.RoleID
GO
