
CREATE VIEW [dbo].[vSchools]
AS
SELECT     SchoolCode AS id, ShortDescription, LongDescription, Abbreviation
FROM         dbo.Schools
GO
