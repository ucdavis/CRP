CREATE TABLE [dbo].[QuestionTypes] (
    [id]               INT          IDENTITY (1, 1) NOT NULL,
    [Name]             VARCHAR (50) NOT NULL,
    [HasOptions]       BIT          CONSTRAINT [DF_QuestionTypes_HasOptions] DEFAULT ((0)) NOT NULL,
    [ExtendedProperty] BIT          NULL,
    CONSTRAINT [PK_QuestionTypes] PRIMARY KEY CLUSTERED ([id] ASC)
);

