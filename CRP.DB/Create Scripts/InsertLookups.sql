 USE CRP
 
 -- //////////////////////////////////////////////////////////////////////////////////////
 -- Insert the question types
 -- //////////////////////////////////////////////////////////////////////////////////////
 
 IF NOT EXISTS ( select * from QuestionTypes where [name] = 'Text Box' )
 begin
	 INSERT INTO QuestionTypes ([Name], hasOptions, ExtendedProperty)
	 VALUES ('Text Box', 0, 1)
 end
 
IF NOT EXISTS ( select * from QuestionTypes where [name] = 'Text Area' )
begin
	 INSERT INTO QuestionTypes ([Name], hasOptions, ExtendedProperty)
	VALUES ('Text Area', 0, 0)
end
 
IF NOT EXISTS ( select * from QuestionTypes where [name] = 'Boolean' )
begin
	INSERT INTO QuestionTypes ([Name], hasOptions, ExtendedProperty)
	VALUES ('Boolean', 0, 0)
end

IF NOT EXISTS ( select * from QuestionTypes where [name] = 'Radio Buttons' )
begin
	INSERT INTO QuestionTypes ([Name], hasOptions, ExtendedProperty)
	VALUES ('Radio Buttons', 1, 0)
end
 
IF NOT EXISTS ( select * from QuestionTypes where [name] = 'Checkbox List' )
begin
	INSERT INTO QuestionTypes ([Name], hasOptions, ExtendedProperty)
	VALUES ('Checkbox List', 1, 0)
end
 
IF NOT EXISTS ( select * from QuestionTypes where [name] = 'Drop Down' )
begin
	INSERT INTO QuestionTypes ([Name], hasOptions, ExtendedProperty)
	VALUES ('Drop Down', 1, 0)
end

IF NOT EXISTS ( select * from QuestionTypes where [name] = 'Date' )
begin
	INSERT INTO QuestionTypes ([Name], hasOptions, ExtendedProperty)
	VALUES ('Date', 0, 1)
end

-- //////////////////////////////////////////////////////////////////////////////////////
-- Insert the default site master into the display profiles
-- //////////////////////////////////////////////////////////////////////////////////////
IF NOT EXISTS ( select * From displayprofiles where sitemaster = 1 )
begin
	insert into displayprofile([name], sitemaster)
	values ('University of California, Davis', 1)
end

-- //////////////////////////////////////////////////////////////////////////////////////
-- Insert question set for contact information
-- //////////////////////////////////////////////////////////////////////////////////////
IF NOT EXISTS ( select * from QuestionSets where Name = 'Contact Information' )
begin
	INSERT INTO QuestionSets ([Name], SystemReusable)
	VALUES ('Contact Information', 1)
	
	declare @qsid int 
	declare @tbId int 
	
	set @qsid = (select Max(id) from QuestionSets where [Name] = 'Contact Information')
	set @tbId = (select max(id) from QuestionTypes where [name] = 'Text Box')
	
	INSERT INTO Questions ([Name], QuestionTypeId, QuestionSetId, [Order], Required)
	VALUES ('First Name', @tbId, @qsid, 1, 1)
	
	INSERT INTO Questions ([Name], QuestionTypeId, QuestionSetId, [Order], Required)
	VALUES ('Last Name', @tbId, @qsid, 2, 1)
	
	INSERT INTO Questions ([Name], QuestionTypeId, QuestionSetId, [Order], Required)
	VALUES ('Street Address', @tbId, @qsid, 3, 1)

	INSERT INTO Questions ([Name], QuestionTypeId, QuestionSetId, [Order], Required)
	VALUES ('Address Line 2', @tbId, @qsid, 4, 1)	
	
	INSERT INTO Questions ([Name], QuestionTypeId, QuestionSetId, [Order], Required)
	VALUES ('City', @tbId, @qsid, 5, 1)	
	
	INSERT INTO Questions ([Name], QuestionTypeId, QuestionSetId, [Order], Required)
	VALUES ('State', @tbId, @qsid, 6, 1)		
	
	INSERT INTO Questions ([Name], QuestionTypeId, QuestionSetId, [Order], Required)
	VALUES ('Zip Code', @tbId, @qsid, 7, 1)		

	INSERT INTO Questions ([Name], QuestionTypeId, QuestionSetId, [Order], Required)
	VALUES ('Phone Number', @tbId, @qsid, 8, 1)		

	INSERT INTO Questions ([Name], QuestionTypeId, QuestionSetId, [Order], Required)
	VALUES ('Email Address', @tbId, @qsid, 5, 1)		
end
go

-- //////////////////////////////////////////////////////////////////////////////////////
-- Insert the default item report
-- //////////////////////////////////////////////////////////////////////////////////////
IF NOT EXISTS (select * from ItemReports where Name = 'Transaction Summary' )
begin
	INSERT INTO ItemReports ([Name], UserId, SystemReusable)
	VALUES ('Transaction Summary', 2, 1)
	
	declare @irId int
	
	set @irId = (select max(id) from ItemReports where [Name] = 'Transaction Summary')
	
	INSERT INTO ItemReportColumns (ItemReportId, [Order], [Name], Quantity, [Transaction], Property)
	VALUES (@irId, 1, 'First Name', 0, 1, 0)
	
	INSERT INTO ItemReportColumns (ItemReportId, [Order], [Name], Quantity, [Transaction], Property)
	VALUES (@irId, 2, 'Last Name', 0, 1, 0)
	
	INSERT INTO ItemReportColumns (ItemReportId, [Order], [Name], Quantity, [Transaction], Property)
	VALUES (@irId, 3, 'Phone Number', 0, 1, 0)		

	INSERT INTO ItemReportColumns (ItemReportId, [Order], [Name], Quantity, [Transaction], Property)
	VALUES (@irId, 4, 'Email Address', 0, 1, 0)	
	
	INSERT INTO ItemReportColumns (ItemReportId, [Order], [Name], Quantity, [Transaction], Property)
	VALUES (@irId, 5, 'DonationTotal', 0, 0, 1)	
	
	INSERT INTO ItemReportColumns (ItemReportId, [Order], [Name], Quantity, [Transaction], Property)
	VALUES (@irId, 6, 'AmountTotal', 0, 0, 1)	
	
	INSERT INTO ItemReportColumns (ItemReportId, [Order], [Name], Quantity, [Transaction], Property)
	VALUES (@irId, 7, 'Total', 0, 0, 1)		
end