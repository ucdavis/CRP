
CREATE Procedure [dbo].[usp_getUsersInRole]
	-- Add the parameters for the stored procedure here
	@AppName nvarchar(50), 
	@RoleName nvarchar(50)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
SELECT     LoginID
FROM         Roles INNER JOIN
                      Permissions ON Roles.RoleID = Permissions.RoleID INNER JOIN
                      Users ON Permissions.UserID = Users.UserID INNER JOIN
                      Applications ON Permissions.ApplicationID = Applications.ApplicationID
WHERE     (Roles.Role = @RoleName) AND (Applications.Name = @AppName) AND (Users.Inactive = 0) AND (Roles.Inactive = 0) AND (Permissions.Inactive = 0) 
                      AND (Applications.Inactive = 0)
END