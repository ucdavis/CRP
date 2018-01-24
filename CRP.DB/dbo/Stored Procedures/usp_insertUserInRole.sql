
CREATE Procedure [dbo].[usp_insertUserInRole]
	-- Add the parameters for the stored procedure here
	@LoginID nvarchar(8), 
	@RoleName nvarchar(50),
	@AppName nvarchar(50),
	@AddedBy nvarchar(8)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
--Assume that the user is not already in the role

DECLARE @UID int --get the username
DECLARE @AppID int --get the appid
DECLARE @RID int --get the roleID
DECLARE @AddedByID int --get the AddedBy username

    -- Insert statements for procedure here
SET @UID = ( SELECT UserID FROM Users WHERE LoginID = @LoginID )

SET @AddedByID = ( SELECT UserID FROM USers WHERE LoginID = @AddedBy )

--now we have the uid, find appid
SET @AppID = ( SELECT TOP 1 ApplicationID FROM Applications
					WHERE ( Name = @AppName ) )

SET @RID = ( SELECT  RoleID FROM  Roles 
				WHERE  (Role = @RoleName) )

--now we have the role, give the user permission to it

INSERT INTO Permissions
                      (UserID, ApplicationID, RoleID,Inactive)
VALUES     (@UID,@AppID,@RID,0)

--Now insert the tracking information

DECLARE @PermissionID int

--Get the inserted PermissionID
SET @PermissionID = (SELECT SCOPE_IDENTITY() AS 'PermissionID')

-- Now Get the TrackingTypeID for "role" and action for 'delete' 
DECLARE @TrackingActionID int
DECLARE @TrackingTypeID int

SET @TrackingActionID = (SELECT TrackingActionID FROM TrackingActions WHERE TrackingAction = 'Add')
SET @TrackingTypeID = (SELECT TrackingTypeID FROM TrackingTypes WHERE TrackingType = 'Role')

INSERT INTO Tracking
                      (TrackingTypeID, TrackingActionID, TrackingUserName, TrackingActionDate, Comments)
VALUES     (@TrackingTypeID, @TrackingActionID,@AddedBy, GETDATE(), 'Role ' + @RID + ' Added For ' + @LoginID)

END