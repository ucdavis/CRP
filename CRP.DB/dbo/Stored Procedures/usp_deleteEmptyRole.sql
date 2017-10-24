
CREATE Procedure [dbo].[usp_deleteEmptyRole]

	-- Add the parameters for the stored procedure here
	@RoleName nvarchar(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

SELECT     LoginID
FROM         Users INNER JOIN
                      Permissions ON Users.UserID = Permissions.UserID INNER JOIN
                      Roles ON Permissions.RoleID = Roles.RoleID
WHERE     (Roles.Role = @RoleName)

IF @@ROWCOUNT = 0
	BEGIN
		UPDATE    Roles
		SET              Inactive = 1
		WHERE     (Role = @RoleName) AND (Inactive = 0)
	END

END