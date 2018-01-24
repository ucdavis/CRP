-- =============================================
-- Author:		Ken Taylor
-- Create date: February 7, 2013
-- Description:	Given an Application Abbreviation, 
-- return a list of Users and their respective Units for the application provided
-- =============================================

CREATE FUNCTION [dbo].[udf_Catbert3_vUserUnits]
(
	@applicationsAbbr nvarchar(50)
)
RETURNS 
@UserUnit TABLE 
(
	UnitId int not null, UserID int not null, Inactive bit not null
)
AS
BEGIN
	-- Fill the table variable with the rows for your result set
	INSERT INTO @UserUnit
	SELECT        TOP (100) PERCENT unitAssociations.UnitID, unitAssociations.UserID, unitassociations.Inactive
	FROM            dbo.UnitAssociations AS unitAssociations INNER JOIN
                         dbo.Applications AS applications ON unitAssociations.ApplicationID = applications.ApplicationID
	WHERE        (applications.Abbr = @applicationsAbbr) --AND (unitAssociations.Inactive = 0)
	ORDER BY unitAssociations.UserID, unitAssociations.UnitID
	
	RETURN 
END