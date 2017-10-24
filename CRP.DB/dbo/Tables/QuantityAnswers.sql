CREATE TABLE [dbo].[QuantityAnswers] (
    [id]            INT              IDENTITY (1, 1) NOT NULL,
    [TransactionId] INT              NOT NULL,
    [QuestionSetId] INT              NOT NULL,
    [QuestionId]    INT              NOT NULL,
    [QuantityId]    UNIQUEIDENTIFIER NOT NULL,
    [Answer]        VARCHAR (MAX)    NOT NULL,
    CONSTRAINT [PK_QuantityAnswers] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_QuantityAnswers_Questions] FOREIGN KEY ([QuestionId]) REFERENCES [dbo].[Questions] ([id]),
    CONSTRAINT [FK_QuantityAnswers_QuestionSets] FOREIGN KEY ([QuestionSetId]) REFERENCES [dbo].[QuestionSets] ([id]),
    CONSTRAINT [FK_QuantityAnswers_Transactions] FOREIGN KEY ([TransactionId]) REFERENCES [dbo].[Transactions] ([id])
);

