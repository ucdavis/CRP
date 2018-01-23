CREATE VIEW dbo.[vUsers2Old]
AS
SELECT     Catbert3.dbo.Users.UserID AS id, Catbert3.dbo.Users.LoginID, Catbert3.dbo.Users.Email, Catbert3.dbo.Users.Phone, Catbert3.dbo.Users.FirstName, 
                      Catbert3.dbo.Users.LastName, Catbert3.dbo.Users.EmployeeID, Catbert3.dbo.Users.SID, Catbert3.dbo.Users.UserKey, 
                      dbo.vActiveUsersForCrp.UserID AS ActiveUserId
FROM         dbo.vActiveUsersForCrp RIGHT OUTER JOIN
                      Catbert3.dbo.Users ON dbo.vActiveUsersForCrp.UserID = Catbert3.dbo.Users.UserID