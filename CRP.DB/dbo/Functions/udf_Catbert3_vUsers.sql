-- =============================================
-- Author:		Ken Taylor
-- Create date: February 7, 2013
-- Description:	Given an Application Abbreviation, 
-- return a list of Users for the application provided
-- =============================================
CREATE FUNCTION [dbo].[udf_Catbert3_vUsers]
(
	@applicationsAbbr nvarchar(50)
)
RETURNS 
@Users TABLE 
(
	UserID int not null, LoginID varchar(10) not null, FirstName nvarchar(50), LastName nvarchar(50), EmployeeID varchar(9), StudentID varchar(9), UserImage nvarchar(50), SID nvarchar(50), UserKey uniqueidentifier not null, Email nvarchar(50) not null, Phone varchar(50), Inactive bit not null
)
AS
BEGIN
	INSERT INTO @Users
	SELECT DISTINCT 
		users.UserID, LoginID, FirstName, LastName, EmployeeID, StudentID, UserImage, SID, UserKey, Email, Phone, permissions.Inactive
	FROM 
		dbo.Users AS users 
	INNER JOIN 
		dbo.Permissions permissions ON permissions.UserID = users.UserID
	INNER JOIN 
		dbo.Applications AS apps ON permissions.ApplicationID = apps.ApplicationID
	--INNER JOIN 
	--	dbo.Roles AS roles ON permissions.RoleID = roles.RoleID
	WHERE 
		apps.Abbr LIKE @applicationsAbbr
	ORDER BY 
		LastName, FirstName
	
	RETURN 
END