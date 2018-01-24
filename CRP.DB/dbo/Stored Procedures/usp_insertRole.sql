
CREATE Procedure [dbo].[usp_insertRole]
	-- Add the parameters for the stored procedure here
	@RoleName nvarchar(50)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
INSERT INTO Roles
                      (Role, Inactive)
VALUES     (@RoleName, 0)

END