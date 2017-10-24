CREATE TABLE [dbo].[QuestionOptions] (
    [id]         INT           IDENTITY (1, 1) NOT NULL,
    [Name]       VARCHAR (200) NOT NULL,
    [QuestionId] INT           NOT NULL,
    CONSTRAINT [PK_QuestionOptions] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_QuestionOptions_Questions] FOREIGN KEY ([QuestionId]) REFERENCES [dbo].[Questions] ([id])
);

