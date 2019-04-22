
CREATE VIEW [dbo].[vUsers2]
AS
SELECT     Users.UserID AS id, Users.LoginID, Users.Email, Users.Phone, Users.FirstName, 
                      Users.LastName, Users.EmployeeID, Users.SID, Users.UserKey, 
                      dbo.vActiveUsersForCrp.UserID AS ActiveUserId
FROM         vActiveUsersForCrp RIGHT OUTER JOIN
                      Users ON dbo.vActiveUsersForCrp.UserID = Users.UserID
GO
