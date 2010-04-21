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