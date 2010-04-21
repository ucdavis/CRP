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
--IF NOT EXISTS ( select * From displayprofiles where sitemaster = 1 )
--begin
--	insert into displayprofile([name], sitemaster)
--	values ('University of California, Davis', 1)
--end

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
	VALUES ('Address Line 2', @tbId, @qsid, 4, 0)	
	
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
	declare @quesetionSetId int
	
	set @quesetionSetId = (select Max(id) from QuestionSets where [Name] = 'Contact Information')	
	set @irId = (select max(id) from ItemReports where [Name] = 'Transaction Summary')
	
	INSERT INTO ItemReportColumns (ItemReportId, [Order], [Name], Quantity, [Transaction], Property, QuestionSetId)
	VALUES (@irId, 1, 'First Name', 0, 1, 0, @quesetionSetId)
	
	INSERT INTO ItemReportColumns (ItemReportId, [Order], [Name], Quantity, [Transaction], Property, QuestionSetId)
	VALUES (@irId, 2, 'Last Name', 0, 1, 0, @quesetionSetId)
	
	INSERT INTO ItemReportColumns (ItemReportId, [Order], [Name], Quantity, [Transaction], Property, QuestionSetId)
	VALUES (@irId, 3, 'Phone Number', 0, 1, 0, @quesetionSetId)		

	INSERT INTO ItemReportColumns (ItemReportId, [Order], [Name], Quantity, [Transaction], Property, QuestionSetId)
	VALUES (@irId, 4, 'Email Address', 0, 1, 0, @quesetionSetId)	
	
	INSERT INTO ItemReportColumns (ItemReportId, [Order], [Name], Quantity, [Transaction], Property, QuestionSetId)
	VALUES (@irId, 5, 'DonationTotal', 0, 0, 1, @quesetionSetId)	
	
	INSERT INTO ItemReportColumns (ItemReportId, [Order], [Name], Quantity, [Transaction], Property, QuestionSetId)
	VALUES (@irId, 6, 'AmountTotal', 0, 0, 1, @quesetionSetId)	
	
	INSERT INTO ItemReportColumns (ItemReportId, [Order], [Name], Quantity, [Transaction], Property, QuestionSetId)
	VALUES (@irId, 7, 'Total', 0, 0, 1, @quesetionSetId)		
end

-- //////////////////////////////////////////////////////////////////////////////////////
-- Insert the default validators
-- //////////////////////////////////////////////////////////////////////////////////////
INSERT INTO [dbo].[Validators]([id], [Name], [Class], [RegEx], [ErrorMessage])
  VALUES(1, 'Required', 'required', '^.+$', '{0} is a required field.')
GO
INSERT INTO [dbo].[Validators]([id], [Name], [Class], [RegEx], [ErrorMessage])
  VALUES(2, 'Email', 'email', '^((([a-z]|\d|[!#\$%&''\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&''\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$', '{0} is not a valid email.')
GO
INSERT INTO [dbo].[Validators]([id], [Name], [Class], [RegEx], [ErrorMessage])
  VALUES(3, 'Url', 'url', '^(https?|ftp):\/\/(((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&''\(\)\*\+,;=]|:)*@)?(((\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5]))|((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?)(:\d*)?)(\/((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&''\(\)\*\+,;=]|:|@)+(\/(([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&''\(\)\*\+,;=]|:|@)*)*)?)?(\?((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&''\(\)\*\+,;=]|:|@)|[\uE000-\uF8FF]|\/|\?)*)?(\#((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&''\(\)\*\+,;=]|:|@)|\/|\?)*)?$', '{0} is not a valid url.')
GO
INSERT INTO [dbo].[Validators]([id], [Name], [Class], [RegEx], [ErrorMessage])
  VALUES(4, 'Date', 'date', '^\d{4}[\/-]\d{1,2}[\/-]\d{1,2}$', '{0} is not a valid date.')
GO
INSERT INTO [dbo].[Validators]([id], [Name], [Class], [RegEx], [ErrorMessage])
  VALUES(5, 'Phone Number', 'phoneUS', '^\(?[\d]{3}\)?[\s-]?[\d]{3}[\s-]?[\d]{4}$', '{0} is not a valid phone number.')
GO
