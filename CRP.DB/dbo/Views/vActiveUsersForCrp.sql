
CREATE VIEW [dbo].[vActiveUsersForCrp]
AS
SELECT DISTINCT Permissions.UserID
FROM         Permissions INNER JOIN
                      Applications ON Permissions.ApplicationID = Applications.ApplicationID
WHERE     (Applications.Abbr = N'CRP') AND (Permissions.Inactive = 0)
GO
