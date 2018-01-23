CREATE VIEW dbo.[vActiveUsersForCrpOld]
AS
SELECT DISTINCT Catbert3.dbo.Permissions.UserID
FROM            Catbert3.dbo.Permissions INNER JOIN
                         Catbert3.dbo.Applications ON Catbert3.dbo.Permissions.ApplicationID = Catbert3.dbo.Applications.ApplicationID
WHERE        (Catbert3.dbo.Applications.Abbr = N'CRP') AND (Catbert3.dbo.Permissions.Inactive = 0)