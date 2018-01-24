CREATE Procedure [dbo].[usp_getRolesInAppByLoginID]
	-- Add the parameters for the stored procedure here
	@AppName nvarchar(50),
	@LoginID nvarchar(8)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here

SELECT     Roles.Role
FROM         Applications INNER JOIN
                      Permissions ON Applications.ApplicationID = Permissions.ApplicationID INNER JOIN
                      Roles ON Permissions.RoleID = Roles.RoleID INNER JOIN
                      Users ON Permissions.UserID = Users.UserID
WHERE     (Users.LoginID = @LoginID) AND (Applications.Name = @AppName) AND (Applications.Inactive = 0) AND (Permissions.Inactive = 0) AND 
                      (Roles.Inactive = 0) AND (Users.Inactive = 0)
END