 USE CRP
 
 IF NOT EXISTS ( select * from QuestionTypes where [name] = 'Text Box' )
 begin
	 INSERT INTO QuestionTypes ([Name], hasOptions)
	 VALUES ('Text Box', 0)
 end
 
IF NOT EXISTS ( select * from QuestionTypes where [name] = 'Text Area' )
begin
	 INSERT INTO QuestionTypes ([Name], hasOptions)
	VALUES ('Text Area', 0)
end
 
IF NOT EXISTS ( select * from QuestionTypes where [name] = 'Boolean' )
begin
	INSERT INTO QuestionTypes ([Name], hasOptions)
	VALUES ('Boolean', 0)
end

IF NOT EXISTS ( select * from QuestionTypes where [name] = 'Radio Buttons' )
begin
	INSERT INTO QuestionTypes ([Name], hasOptions)
	VALUES ('Radio Buttons', 1)
end
 
IF NOT EXISTS ( select * from QuestionTypes where [name] = 'Checkbox List' )
begin
	INSERT INTO QuestionTypes ([Name], hasOptions)
	VALUES ('Checkbox List', 1)
end
 
IF NOT EXISTS ( select * from QuestionTypes where [name] = 'Drop Down' )
begin
	INSERT INTO QuestionTypes ([Name], hasOptions)
	VALUES ('Drop Down', 1)
end

IF NOT EXISTS ( select * from QuestionTypes where [name] = 'Date' )
begin
	INSERT INTO QuestionTypes ([Name], hasOptions)
	VALUES ('Date', 0)
end