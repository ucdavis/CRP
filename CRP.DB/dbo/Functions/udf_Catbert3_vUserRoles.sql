-- =============================================
-- Author:		Ken Taylor
-- Create date: February 7, 2013
-- Description:	Given an Application Abbreviation, 
-- return a list of User Roles for the application provided
-- =============================================
CREATE FUNCTION [dbo].[udf_Catbert3_vUserRoles]
(
	@applicationsAbbr nvarchar(50)
)
RETURNS 
@UserRoles TABLE 
(
	UserID int not null,
	RoleID int not null,
	Inactive bit not null
)
AS
BEGIN
	INSERT INTO @UserRoles
	SELECT        TOP (100) PERCENT permissions.UserID, permissions.RoleID, permissions.Inactive
	FROM            dbo.Permissions AS permissions INNER JOIN
                         dbo.Applications AS applications ON permissions.ApplicationID = applications.ApplicationID
	WHERE        (applications.Abbr = @applicationsAbbr) --AND (permissions.Inactive = 0)
	ORDER BY permissions.UserID, permissions.RoleID
	
	RETURN 
END