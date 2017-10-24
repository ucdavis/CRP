CREATE TABLE [dbo].[ItemQuestionSets] (
    [id]               INT IDENTITY (1, 1) NOT NULL,
    [ItemId]           INT NOT NULL,
    [QuestionSetId]    INT NOT NULL,
    [TransactionLevel] BIT CONSTRAINT [DF_ItemQuestionSets_TransactionLevel] DEFAULT ((0)) NOT NULL,
    [QuantityLevel]    BIT CONSTRAINT [DF_ItemQuestionSets_QuantityLevel] DEFAULT ((0)) NOT NULL,
    [Order]            INT NOT NULL,
    [Required]         BIT CONSTRAINT [DF_ItemQuestionSets_Required] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_ItemQuestionSets] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_ItemQuestionSets_Items] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Items] ([id]),
    CONSTRAINT [FK_ItemQuestionSets_QuestionSets] FOREIGN KEY ([QuestionSetId]) REFERENCES [dbo].[QuestionSets] ([id])
);

