
CREATE Procedure [dbo].[usp_deleteUserFromRole]
	-- Add the parameters for the stored procedure here
	@AppName nvarchar(50),
	@RoleName nvarchar(50),
	@LoginID nvarchar(8),
	@DeletedBy nvarchar(8)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
DECLARE @UID int --get the username
DECLARE @AppID int --get the appid
DECLARE @RID int --get the roleID
DECLARE @DeletedByID int --get the DeletedBy username

    -- Insert statements for procedure here
SET @UID = (SELECT UserID FROM Users WHERE LoginID = @LoginID)

SET @DeletedByID = ( SELECT UserID FROM Users WHERE LoginID = @DeletedBy )

--now we have the uid, find appid
SET @AppID = ( SELECT TOP 1 ApplicationID FROM Applications WHERE Name = @AppName )

SET @RID = ( SELECT RoleID FROM  Roles WHERE  (Role = @RoleName) )

-- Grab the @PermissionID that we are to track
DECLARE @PermissionID int

SET @PermissionID = (SELECT PermissionID 
						FROM Permissions 
						WHERE (UserID = @UID) AND (ApplicationID = @AppID) AND (RoleID = @RID) )

-- Now we can set that permission as false
UPDATE    Permissions
	SET              Inactive = 1
	WHERE PermissionID = @PermissionID

-- Now Insert the tracking information

-- Now Get the TrackingTypeID for "role" and action for 'delete' 
DECLARE @TrackingActionID int
DECLARE @TrackingTypeID int

SET @TrackingActionID = (SELECT TrackingActionID FROM TrackingActions WHERE TrackingAction = 'Delete')
SET @TrackingTypeID = (SELECT TrackingTypeID FROM TrackingTypes WHERE TrackingType = 'Role')

INSERT INTO Tracking
                      (TrackingTypeID, TrackingActionID, TrackingUserName, TrackingActionDate, Comments)
VALUES     (@TrackingTypeID, @TrackingActionID,@DeletedByID, GETDATE(), 'Role ' + @RID + ' Removed For ' + @LoginID)

END