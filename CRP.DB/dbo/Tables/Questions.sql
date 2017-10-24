CREATE TABLE [dbo].[Questions] (
    [id]             INT           IDENTITY (1, 1) NOT NULL,
    [Name]           VARCHAR (200) NOT NULL,
    [QuestionTypeId] INT           NOT NULL,
    [QuestionSetId]  INT           NOT NULL,
    [Order]          INT           NOT NULL,
    CONSTRAINT [PK_Questions] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_Questions_QuestionSets] FOREIGN KEY ([QuestionSetId]) REFERENCES [dbo].[QuestionSets] ([id]),
    CONSTRAINT [FK_Questions_QuestionTypes] FOREIGN KEY ([QuestionTypeId]) REFERENCES [dbo].[QuestionTypes] ([id])
);

