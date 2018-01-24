CREATE PROCEDURE [dbo].[usp_ItemsSearch] 
	-- Add the parameters for the stored procedure here
	@query varchar(256)
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
 
SELECT DISTINCT Items.*
FROM ExtendedPropertyAnswers RIGHT OUTER JOIN
                      Items ON ExtendedPropertyAnswers.ItemId = Items.id LEFT OUTER JOIN
                      Tags INNER JOIN
                      ItemsXTags ON Tags.id = ItemsXTags.TagId ON Items.id = ItemsXTags.ItemId
WHERE
	freetext(Items.Name, @query) OR
	freetext(Items.Description, @query) OR
	freetext(Tags.Name, @query) OR
	freetext(ExtendedPropertyAnswers.Answer, @query)

END