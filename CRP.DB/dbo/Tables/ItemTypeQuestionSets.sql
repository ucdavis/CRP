CREATE TABLE [dbo].[ItemTypeQuestionSets] (
    [id]               INT IDENTITY (1, 1) NOT NULL,
    [ItemTypeId]       INT NOT NULL,
    [QuestionSetId]    INT NOT NULL,
    [TransactionLevel] BIT CONSTRAINT [DF_ItemTypeQuestionSets_TransactionLevel] DEFAULT ((0)) NOT NULL,
    [QuantityLevel]    BIT CONSTRAINT [DF_ItemTypeQuestionSets_QuantityLevel] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_ItemTypeQuestionSets] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_ItemTypesXQuestionSets_ItemTypes] FOREIGN KEY ([ItemTypeId]) REFERENCES [dbo].[ItemTypes] ([id]),
    CONSTRAINT [FK_ItemTypesXQuestionSets_QuestionSets] FOREIGN KEY ([QuestionSetId]) REFERENCES [dbo].[QuestionSets] ([id])
);

