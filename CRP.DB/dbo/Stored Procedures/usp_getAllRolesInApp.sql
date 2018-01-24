
CREATE Procedure [dbo].[usp_getAllRolesInApp]
	-- Add the parameters for the stored procedure here
	@AppName nvarchar(50)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
SELECT     Roles.Role
FROM         Applications INNER JOIN
                      ApplicationRoles ON Applications.ApplicationID = ApplicationRoles.ApplicationID INNER JOIN
                      Roles ON ApplicationRoles.RoleID = Roles.RoleID
WHERE     (Applications.Name = @AppName) AND (Roles.Inactive = 0) AND (Applications.Inactive = 0)

END