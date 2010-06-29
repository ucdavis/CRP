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

IF NOT EXISTS ( select * from QuestionTypes where [name] = 'No Answer' )
begin
	INSERT INTO QuestionTypes ([Name], hasOptions, ExtendedProperty)
	VALUES ('No Answer', 0, 0)
end
GO

-- //////////////////////////////////////////////////////////////////////////////////////
-- Insert the default validators
-- //////////////////////////////////////////////////////////////////////////////////////
INSERT INTO [dbo].[Validators]([Name], [Class], [RegEx], [ErrorMessage])
  VALUES('Required', 'required', '^.+$', '{0} is a required field.')
GO
INSERT INTO [dbo].[Validators]([Name], [Class], [RegEx], [ErrorMessage])
  VALUES('Email', 'email', '(^((([a-z]|\d|[!#\$%&''\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&''\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$){1}|^$', '{0} is not a valid email.')
GO
INSERT INTO [dbo].[Validators]([Name], [Class], [RegEx], [ErrorMessage])
  VALUES('Url', 'url', '(^(https?|ftp):\/\/(((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&''\(\)\*\+,;=]|:)*@)?(((\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5]))|((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?)(:\d*)?)(\/((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&''\(\)\*\+,;=]|:|@)+(\/(([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&''\(\)\*\+,;=]|:|@)*)*)?)?(\?((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&''\(\)\*\+,;=]|:|@)|[\uE000-\uF8FF]|\/|\?)*)?(\#((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&''\(\)\*\+,;=]|:|@)|\/|\?)*)?$){1}|^$', '{0} is not a valid url.')
GO
INSERT INTO [dbo].[Validators]([Name], [Class], [RegEx], [ErrorMessage])
  VALUES('Date', 'date', '(^(((0?[1-9]|1[012])[-\/](0?[1-9]|1\d|2[0-8])|(0?[13456789]|1[012])[-\/](29|30)|(0?[13578]|1[02])[-\/]31)[-\/](19|[2-9]\d)\d{2}|0?2[-\/]29[-\/]((19|[2-9]\d)(0[48]|[2468][048]|[13579][26])|(([2468][048]|[3579][26])00)))$){1}|^$', '{0} is not a valid date.')
GO
INSERT INTO [dbo].[Validators]([Name], [Class], [RegEx], [ErrorMessage])
  VALUES('Phone Number', 'phoneUS', '(^\(?[\d]{3}\)?[\s-]?[\d]{3}[\s-]?[\d]{4}$){1}|^$', '{0} is not a valid phone number.')
GO

-- //////////////////////////////////////////////////////////////////////////////////////
-- Insert question set for contact information
-- //////////////////////////////////////////////////////////////////////////////////////
IF NOT EXISTS ( select * from QuestionSets where Name = 'Contact Information' )
begin
	INSERT INTO QuestionSets ([Name], SystemReusable)
	VALUES ('Contact Information', 1)
	
	declare @qsid int 
	declare @tbId int 
	declare @ddId int	-- drop down id

	set @qsid = (select Max(id) from QuestionSets where [Name] = 'Contact Information' and SystemReusable = 1)
	set @tbId = (select max(id) from QuestionTypes where [name] = 'Text Box')
	set @ddId = (select max(id) from QuestionTypes where [name] = 'Drop Down')
	
	declare @required int	-- required validator
	declare @phonenumber int	-- phone validator
	declare @email int		-- email validator
	declare @qid int		-- question id to add validation
	
	set @required = (select max(id) from validators where [name] = 'Required')
	set @phonenumber = (select max(id) from validators where [name] = 'Phone Number')
	set @email = (select max(id) from validators where [name] = 'Email')
	
	INSERT INTO Questions ([Name], QuestionTypeId, QuestionSetId, [Order])
	VALUES ('First Name', @tbId, @qsid, 1)
	
	set @qid = (select max(id) from questions where name = 'First Name')
	insert into QuestionXValidator (QuestionId, ValidatorId) values (@qid, @required)
		
	INSERT INTO Questions ([Name], QuestionTypeId, QuestionSetId, [Order])
	VALUES ('Last Name', @tbId, @qsid, 2)
	
	set @qid = (select max(id) from questions where name = 'Last Name')
	insert into QuestionXValidator (QuestionId, ValidatorId) values (@qid, @required)
	
	INSERT INTO Questions ([Name], QuestionTypeId, QuestionSetId, [Order])
	VALUES ('Street Address', @tbId, @qsid, 3)

	set @qid = (select max(id) from questions where name = 'Street Address')
	insert into QuestionXValidator (QuestionId, ValidatorId) values (@qid, @required)

	INSERT INTO Questions ([Name], QuestionTypeId, QuestionSetId, [Order])
	VALUES ('Address Line 2', @tbId, @qsid, 4)	
	
	INSERT INTO Questions ([Name], QuestionTypeId, QuestionSetId, [Order])
	VALUES ('City', @tbId, @qsid, 5)	
	
	set @qid = (select max(id) from questions where name = 'City')
	insert into QuestionXValidator (QuestionId, ValidatorId) values (@qid, @required)
	
	INSERT INTO Questions ([Name], QuestionTypeId, QuestionSetId, [Order])
	VALUES ('State', @ddId, @qsid, 6)		
	
	set @qid = (select max(id) from questions where name = 'State')
	insert into QuestionXValidator (QuestionId, ValidatorId) values (@qid, @required)
	
	INSERT INTO Questions ([Name], QuestionTypeId, QuestionSetId, [Order])
	VALUES ('Zip Code', @tbId, @qsid, 7)		
	
	set @qid = (select max(id) from questions where name = 'Zip Code')
	insert into QuestionXValidator (QuestionId, ValidatorId) values (@qid, @required)

	INSERT INTO Questions ([Name], QuestionTypeId, QuestionSetId, [Order])
	VALUES ('Phone Number', @tbId, @qsid, 8)		
	
	set @qid = (select max(id) from questions where name = 'Phone Number')
	insert into QuestionXValidator (QuestionId, ValidatorId) values (@qid, @required)
	insert into QuestionXValidator (QuestionId, ValidatorId) values (@qid, @phonenumber)

	INSERT INTO Questions ([Name], QuestionTypeId, QuestionSetId, [Order])
	VALUES ('Email Address', @tbId, @qsid, 9)		
	
	set @qid = (select max(id) from questions where name = 'Email Address')
	insert into QuestionXValidator (QuestionId, ValidatorId) values (@qid, @required)
	insert into QuestionXValidator (QuestionId, ValidatorId) values (@qid, @email)

	-- get the state drop down question and insert the options
	declare @stateId int
	
	set @stateId = (select max(id) from Questions where [name] = 'State' and QuestionSetId = @qsid)

	INSERT INTO QuestionOptions (Name, QuestionId) Values('AL', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('AK', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('AZ', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('AR', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('CA', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('CO', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('CT', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('DE', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('FL', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('GA', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('HI', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('ID', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('IL', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('IN', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('IA', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('KS', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('KY', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('LA', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('ME', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('MD', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('MA', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('MI', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('MN', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('MS', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('MO', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('MT', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('NE', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('NV', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('NH', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('NJ', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('NM', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('NY', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('NC', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('ND', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('OH', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('OK', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('OR', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('PA', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('RI', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('SC', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('SD', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('TN', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('TX', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('UT', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('VT', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('VA', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('WA', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('WV', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('WI', @stateId)
	INSERT INTO QuestionOptions (Name, QuestionId) Values('WY', @stateId)
	
	end
GO

-- //////////////////////////////////////////////////////////////////////////////////////
-- Insert the default site master into the display profiles
-- //////////////////////////////////////////////////////////////////////////////////////
--IF NOT EXISTS ( select * From displayprofiles where sitemaster = 1 )
--begin
--	insert into displayprofile([name], sitemaster)
--	values ('University of California, Davis', 1)
--end

-- //////////////////////////////////////////////////////////////////////////////////////
-- Insert the default item report
-- Be sure to update the user id if that userid doesn't exist
-- //////////////////////////////////////////////////////////////////////////////////////
IF NOT EXISTS (select * from ItemReports where Name = 'Transaction Summary' and SystemReusable = 1 )
begin
	INSERT INTO ItemReports ([Name], UserId, SystemReusable)
	VALUES ('Transaction Summary', 1, 1)
	
	declare @irId int
	declare @quesetionSetId int
	
	set @quesetionSetId = (select Max(id) from QuestionSets where [Name] = 'Contact Information')	
	set @irId = (select max(id) from ItemReports where [Name] = 'Transaction Summary' and SystemReusable = 1)
	
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

GO

-- //////////////////////////////////////////////////////////////////////////////////////
-- Insert the default item report
-- Be sure to update the user id if that userid doesn't exist
-- //////////////////////////////////////////////////////////////////////////////////////
IF NOT EXISTS (select * from ItemReports where Name = 'Checks' and SystemReusable = 1 )
begin
	INSERT INTO ItemReports ([Name], UserId, SystemReusable)
	VALUES ('Checks', 1, 1)
	
	declare @irId int
	set @irId = (select max(id) from ItemReports where [Name] = 'Checks' and SystemReusable = 1)
	
	INSERT INTO ItemReportColumns (ItemReportId, [Order], [Name], Quantity, [Transaction], Property, QuestionSetId)
	VALUES (@irId, 1, 'Checks_Amount', 0, 0, 1, null)		

	INSERT INTO ItemReportColumns (ItemReportId, [Order], [Name], Quantity, [Transaction], Property, QuestionSetId)
	VALUES (@irId, 1, 'Checks_CheckNumber', 0, 0, 1, null)		
	
	INSERT INTO ItemReportColumns (ItemReportId, [Order], [Name], Quantity, [Transaction], Property, QuestionSetId)
	VALUES (@irId, 1, 'Checks_DateReceived', 0, 0, 1, null)		
	
	INSERT INTO ItemReportColumns (ItemReportId, [Order], [Name], Quantity, [Transaction], Property, QuestionSetId)
	VALUES (@irId, 1, 'Checks_Notes', 0, 0, 1, null)		
	
	INSERT INTO ItemReportColumns (ItemReportId, [Order], [Name], Quantity, [Transaction], Property, QuestionSetId)
	VALUES (@irId, 1, 'Checks_Payee', 0, 0, 1, null)		
	
	INSERT INTO ItemReportColumns (ItemReportId, [Order], [Name], Quantity, [Transaction], Property, QuestionSetId)
	VALUES (@irId, 1, 'Checks_TransactionId', 0, 0, 1, null)		
		
end
GO