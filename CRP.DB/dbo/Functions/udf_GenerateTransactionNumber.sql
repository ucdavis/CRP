-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[udf_GenerateTransactionNumber]
(
	@year int,
	@id int
)
RETURNS varchar(max)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @transNumber varchar(max)

	set @transNumber = convert(varchar, @year) + '-'
	
	if (@id >= 100000)
		begin
			set @transNumber = @transNumber + convert(varchar, @id)			
		end
	else
		begin
			set @transNumber = @transNumber + replicate('0', 6-Len(convert(varchar, @id))) + convert(varchar,@id)
		end
	

	-- Return the result of the function
	RETURN @transNumber

END