
CREATE VIEW [dbo].[vUnitAssociations]
AS
SELECT     UnitAssociationID, UnitID, UserID, ApplicationID, Inactive
FROM         UnitAssociations
WHERE     (ApplicationID IN
                          (SELECT     ApplicationID
                            FROM          Applications
                            WHERE      (Abbr = 'CRP'))) AND (Inactive = 0)
GO
