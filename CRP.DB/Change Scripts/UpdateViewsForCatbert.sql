USE [CRP]
GO

/****** Object:  View [dbo].[vActiveUsersForCrp]    Script Date: 11/10/2016 10:54:52 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER VIEW [dbo].[vActiveUsersForCrp]
AS
SELECT DISTINCT Permissions.UserID
FROM         Permissions INNER JOIN
                      Applications ON Catbert3.dbo.Permissions.ApplicationID = Applications.ApplicationID
WHERE     (Applications.Abbr = N'CRP') AND (Permissions.Inactive = 0)

GO


USE [CRP]
GO

/****** Object:  View [dbo].[vSchools]    Script Date: 11/10/2016 10:56:04 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER VIEW [dbo].[vSchools]
AS
SELECT     SchoolCode AS id, ShortDescription, LongDescription, Abbreviation
FROM         dbo.Schools

GO

USE [CRP]
GO

/****** Object:  View [dbo].[vUnitAssociations]    Script Date: 11/10/2016 10:56:36 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER VIEW [dbo].[vUnitAssociations]
AS
SELECT     UnitAssociationID, UnitID, UserID, ApplicationID, Inactive
FROM         UnitAssociations
WHERE     (ApplicationID IN
                          (SELECT     ApplicationID
                            FROM          Applications
                            WHERE      (Abbr = 'CRP'))) AND (Inactive = 0)

GO

USE [CRP]
GO

/****** Object:  View [dbo].[vUnits]    Script Date: 11/10/2016 10:57:32 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER VIEW [dbo].[vUnits]
AS
SELECT     UnitID AS id, FullName, ShortName, PPS_Code, FIS_Code, SchoolCode
FROM         Unit

GO

USE [CRP]
GO

/****** Object:  View [dbo].[vUsers]    Script Date: 11/10/2016 10:58:06 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER VIEW [dbo].[vUsers]
AS
SELECT     UserID AS id, LoginID, Email, Phone, FirstName, LastName, EmployeeID, SID, UserKey
FROM         Users
WHERE     (Inactive = 0)

GO

USE [CRP]
GO

/****** Object:  View [dbo].[vUsers2]    Script Date: 11/10/2016 10:58:40 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER VIEW [dbo].[vUsers2]
AS
SELECT     Users.UserID AS id, Users.LoginID, Users.Email, Users.Phone, Users.FirstName, 
                      Users.LastName, Users.EmployeeID, Users.SID, Users.UserKey, 
                      dbo.vActiveUsersForCrp.UserID AS ActiveUserId
FROM         vActiveUsersForCrp RIGHT OUTER JOIN
                      Users ON dbo.vActiveUsersForCrp.UserID = Users.UserID

GO


