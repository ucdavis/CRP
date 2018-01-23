CREATE VIEW [dbo].[vSchoolsOld]
AS
SELECT     SchoolCode AS id, ShortDescription, LongDescription, Abbreviation
FROM         Catbert3.dbo.Schools