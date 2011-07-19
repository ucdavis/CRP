/*
	Specify the item Id to delete
	Note, did not try this on an item that had tranactions, but it should rollback if that is the case
*/


BEGIN TRANSACTION;
GO
Declare @ItemIdToDelete int
set @ItemIdToDelete = xx

DELETE FROM [CRP].[dbo].[MapPins]
      WHERE ItemId = @ItemIdToDelete    

DELETE FROM [CRP].[dbo].[Templates]
	WHERE ItemId = @ItemIdToDelete
	
DELETE	FROM [CRP].[dbo].[ItemsXTags]
	WHERE ItemId = @ItemIdToDelete
	
DELETE	FROM [CRP].[dbo].[ItemReportColumns]
	WHERE [ItemReportId] in (Select [id] FROM [CRP].[dbo].[ItemReports] where ItemId = @ItemIdToDelete)	
    	
DELETE	FROM [CRP].[dbo].[ItemReports]
	WHERE ItemId = @ItemIdToDelete
    
DELETE FROM [CRP].[dbo].[ItemQuestionSets]
    WHERE ItemId = @ItemIdToDelete
          
DELETE FROM [CRP].[dbo].[ExtendedPropertyAnswers]
	WHERE ItemId = @ItemIdToDelete

DELETE FROM [CRP].[dbo].[Editors]	
	WHERE ItemId = @ItemIdToDelete
    	
DELETE FROM [CRP].[dbo].[Coupons]
	WHERE ItemId = @ItemIdToDelete
    	
DELETE FROM [CRP].[dbo].[Items]
      WHERE id = @ItemIdToDelete	
GO

IF (@@ERROR <> 0) BEGIN
	PRINT 'Unexpected error occurred! ROLLLING BACK'
	ROLLBACK TRANSACTION;		
END	
ELSE BEGIN 
	COMMIT TRANSACTION;
			
END	 

GO          




