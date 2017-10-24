CREATE TABLE [dbo].[QuestionXValidator] (
    [QuestionId]  INT NOT NULL,
    [ValidatorId] INT NOT NULL,
    CONSTRAINT [PK_QuestionXValidator] PRIMARY KEY CLUSTERED ([QuestionId] ASC, [ValidatorId] ASC),
    CONSTRAINT [FK_QuestionXValidator_Questions] FOREIGN KEY ([QuestionId]) REFERENCES [dbo].[Questions] ([id]),
    CONSTRAINT [FK_QuestionXValidator_Validators] FOREIGN KEY ([ValidatorId]) REFERENCES [dbo].[Validators] ([id])
);

