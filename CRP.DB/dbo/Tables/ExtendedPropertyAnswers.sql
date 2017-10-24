CREATE TABLE [dbo].[ExtendedPropertyAnswers] (
    [id]                 INT           IDENTITY (1, 1) NOT NULL,
    [Answer]             VARCHAR (MAX) NOT NULL,
    [ItemId]             INT           NOT NULL,
    [ExtendedPropertyId] INT           NOT NULL,
    CONSTRAINT [PK_ExtendedPropertyAnswers] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_ExtendedPropertyAnswers_ExtendedProperties] FOREIGN KEY ([ExtendedPropertyId]) REFERENCES [dbo].[ExtendedProperties] ([id]),
    CONSTRAINT [FK_ExtendedPropertyAnswers_Items] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Items] ([id])
);

