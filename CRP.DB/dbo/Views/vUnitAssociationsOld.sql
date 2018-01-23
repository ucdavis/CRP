CREATE VIEW [dbo].[vUnitAssociationsOld]
AS
SELECT     UnitAssociationID, UnitID, UserID, ApplicationID, Inactive
FROM         Catbert3.dbo.UnitAssociations
WHERE     (ApplicationID IN
                          (SELECT     ApplicationID
                            FROM          Catbert3.dbo.Applications
                            WHERE      (Abbr = 'CRP'))) AND (Inactive = 0)