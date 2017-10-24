CREATE TABLE [dbo].[TransactionAnswers] (
    [id]            INT           IDENTITY (1, 1) NOT NULL,
    [TransactionId] INT           NOT NULL,
    [QuestionSetId] INT           NOT NULL,
    [QuestionId]    INT           NOT NULL,
    [Answer]        VARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_TransactionAnswers] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_TransactionAnswers_Questions] FOREIGN KEY ([QuestionId]) REFERENCES [dbo].[Questions] ([id]),
    CONSTRAINT [FK_TransactionAnswers_QuestionSets] FOREIGN KEY ([QuestionSetId]) REFERENCES [dbo].[QuestionSets] ([id]),
    CONSTRAINT [FK_TransactionAnswers_Transactions] FOREIGN KEY ([TransactionId]) REFERENCES [dbo].[Transactions] ([id])
);

